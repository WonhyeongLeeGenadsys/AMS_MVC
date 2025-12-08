using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InfluxDB.Client;
using Web.Common;

namespace AMS_DATA
{
    static class AddrCodeMap
    {
        public static readonly Dictionary<string, string> ADDR_TO_CODE =
            new Dictionary<string, string>()
            {
                ["YHLU07/SPDC1$ST$PaDschAlm$stVal"] = "ITR001",// 정밀점검 PD 측정값(1)
                ["YHLU07/SPDC2$ST$PaDschAlm$stVal"] = "ITR002",  // 정밀점검 PD 측정값(2)
                //온도값 추가 예정
            };
    }

    class SignalHit
    {
        public string Addr;
        public string Code;
        public float Value;
    }

    class Program
    {
        static void LogInfo(string msg) => Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO]  {msg}");
        static void LogWarn(string msg) => Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [WARN]  {msg}");
        static void LogError(string msg, Exception ex = null)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ERROR] {msg}");
            if (ex != null) Console.WriteLine(ex.ToString());
        }

        static async Task Main(string[] args)
        {
            var vcbChkRepo = new VCBChkRepository();
            var itr1ChkRepo = new ITRChk1Repository();
            var itr2ChkRepo = new ITRChk2Repository();
            var dccbChkRepo = new DCCBChkRepository();
            var dccableChkRepo = new DCCABLEChkRepository();
            var submoduleChkRepo = new SUBMODULEChkRepository();

            var riskRepo = new RiskmatrixRepository();
            var cofRepo = new CoFRepository();
            var itrCalc = new ITRChkScoreCalculator();

            string url = ConfigurationManager.AppSettings["InfluxUrl"];          
            string token = ConfigurationManager.AppSettings["InfluxToken"];      
            string org = ConfigurationManager.AppSettings["InfluxOrg"] ?? "mvdc";
            string bucket = ConfigurationManager.AppSettings["InfluxBucket"] ?? "AMS";
            var client = InfluxDBClientFactory.Create(url, token);

            // 자정 1회 처리 마커
            DateTime lastDailyProcessed = DateTime.MinValue.Date;

            while (true)
            {
                try
                {
                    bool itrHandledThisLoop = false;

                    LogInfo("Influx 수집 시작");
                    var hits = await FetchSignalsWithCodes(client, org, bucket);
                    LogInfo($"Influx 수집 완료: {hits.Count} 개");

                    // 신호 들어온 code별 처리
                    foreach (var grp in hits.GroupBy(h => h.Code))
                    {
                        var code = grp.Key;
                        var value = grp.First().Value;

                        if (code.StartsWith("ITR"))
                        {
                            // ITR: 신호 있을 때만 HI/PoF 계산하여 오늘자 RM Upsert (보통점검 이력이 존재한다면!)
                            if (HasChk(code, itr1ChkRepo, itr2ChkRepo))
                            {
                                LogInfo($"ITR 처리 시작: {code}, value={value}");
                                UpdateItrRiskOnly(itr1ChkRepo, itr2ChkRepo, itrCalc, cofRepo, riskRepo, code, value);
                                LogInfo($"ITR 처리 끝: {code}");
                                LogInfo($"-------------------------------------------------------------------------------------------------");
                                itrHandledThisLoop = true;
                            }
                            else
                            {
                                LogWarn($"ITR 보통점검 없음 → 스킵: {code}");
                            }
                        }
                        else
                        {
                            // 4종: 보통점검 이력 + 최신 RM이 있을 경우 오늘자로 복사/Upsert
                            if (HasChk(code, vcbChkRepo, dccbChkRepo, dccableChkRepo, submoduleChkRepo))
                            {
                                LogInfo($"4종 RM 오늘자 보장(복사) 시도: {code}");
                                CopyLatestRiskToTodayIfNeeded(riskRepo, code);
                            }
                            else
                            {
                                LogWarn($"4종 보통점검 없음 → 스킵: {code}");
                            }
                        }
                    }

                    // 이번 루프에서 ITR 신호를 하나라도 처리했다면 4종 전체 보정(오늘자 복사) 수행
                    //if (itrHandledThisLoop)
                    //{
                    //    LogInfo("ITR 신호 처리됨 → 4종 (오늘자 복사) 실행");
                    //    CopyAllFourTypesLatestToTodayIfExists(
                    //        riskRepo,
                    //        vcbChkRepo, dccbChkRepo, dccableChkRepo, submoduleChkRepo
                    //    );
                    //}

                    // 자정 1회 확인: 4종 전체 오늘자 Upsert (보통점검 + 최신 RM 있을 때만)
                    if (DateTime.Today > lastDailyProcessed)
                    {
                        lastDailyProcessed = DateTime.Today;
                        LogInfo("00시 4종 오늘자 복사");
                        CopyAllFourTypesLatestToTodayIfExists(
                            riskRepo,
                            vcbChkRepo, dccbChkRepo, dccableChkRepo, submoduleChkRepo
                        );
                    }
                }
                catch (Exception ex)
                {
                    // 어떤 예외도 서비스 중단 없이 다음 틱에서 재시도
                    LogError("메인 루프 처리 중 예외 발생(루프는 계속 돈다).", ex);
                }

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }

        // Influx에서 신호 읽어 주소 --> (코드, 값)으로 반환 (매핑되지 않은 주소는 무시)
        static async Task<List<SignalHit>> FetchSignalsWithCodes(InfluxDBClient client, string org, string bucket)
        {
            var flux = $@"
            from(bucket: ""{bucket}"")
              |> range(start: -5s)
              |> filter(fn: (r) => r._field == ""value"")
              |> last()";

            var result = new List<SignalHit>();

            try
            {
                var tables = await client.GetQueryApi().QueryAsync(flux, org);

                int total = 0, mapped = 0, parsed = 0;
                foreach (var rec in tables.SelectMany(t => t.Records))
                {
                    total++;

                    if (!rec.Values.TryGetValue("ADDR", out var addrObj) || addrObj == null)
                        continue;

                    var addr = addrObj.ToString();
                    if (string.IsNullOrWhiteSpace(addr))
                        continue;

                    if (!AddrCodeMap.ADDR_TO_CODE.TryGetValue(addr, out var code))
                        continue; // 매핑 안 된 주소는 패스함!
                    mapped++;

                    var valStr = rec.GetValue()?.ToString();
                    if (float.TryParse(valStr, out var v))
                    {
                        result.Add(new SignalHit { Addr = addr, Code = code, Value = v });
                        parsed++;
                    }
                }
                LogInfo($"Flux 결과 처리: total={total}, mapped={mapped}, parsed={parsed}");
            }
            catch (Exception ex)
            {
                LogError("FetchSignalsWithCodes 예외 발생.", ex);
            }

            return result;
        }

        //  보통점검 존재 여부
        // ITR 전용: 보통점검 이력 존재?
        static bool HasChk(string code, ITRChk1Repository itr1Repo, ITRChk2Repository itr2Repo)
        {
            itr1Repo.GetLatestITRChk1ByITRCode(code, out var l1);
            itr2Repo.GetLatestITRChk2ByITRCode(code, out var l2);
            return (l1?.Any() ?? false) || (l2?.Any() ?? false);
        }

        // 4종 전용: 보통점검 이력 존재?
        static bool HasChk(
            string code,
            VCBChkRepository vcbRepo,
            DCCBChkRepository dccbRepo,
            DCCABLEChkRepository dccableRepo,
            SUBMODULEChkRepository submoduleRepo)
        {
            if (code.StartsWith("VCB"))
            {
                vcbRepo.GetLatestVCBChkByVCBCode(code, out var l);
                return l?.Any() ?? false;
            }
            if (code.StartsWith("DCCB"))
            {
                dccbRepo.GetLatestDCCBChkByDCCBCode(code, out var l);
                return l?.Any() ?? false;
            }
            if (code.StartsWith("DCCABLE"))
            {
                dccableRepo.GetLatestDCCABLEChkByDCCABLECode(code, out var l);
                return l?.Any() ?? false;
            }
            if (code.StartsWith("SUBMODULE"))
            {
                submoduleRepo.GetLatestSUBMODULEChkBySUBMODULECode(code, out var l);
                return l?.Any() ?? false;
            }
            return false;
        }

        // ITR: 신호(PD) 반영해서 HI/PoF 계산 --> RiskMatrix 오늘자 Upsert
        static void UpdateItrRiskOnly(
            ITRChk1Repository itr1Repo,
            ITRChk2Repository itr2Repo,
            ITRChkScoreCalculator itrCalc,
            CoFRepository cofRepo,
            RiskmatrixRepository riskRepo,
            string itrCode,
            float pdRaw)
        {
            itr1Repo.GetLatestITRChk1ByITRCode(itrCode, out var l1);
            itr2Repo.GetLatestITRChk2ByITRCode(itrCode, out var l2);

            var latest1 = l1?.OrderBy(x => x.Tbl_Idx).LastOrDefault();
            var latest2 = l2?.OrderBy(x => x.Tbl_Idx).LastOrDefault();
            if (latest1 == null && latest2 == null) return;

            var t1 = latest1 ?? new ITRChk1();
            var t2 = latest2 ?? new ITRChk2();

            // PD: 0 → 1등급, 아니면 5등급
            t2.CHK2_PD = (pdRaw == 0f) ? 1 : 5;

            var (hi, pof) = itrCalc.CalculateHiPofCombined(t1, t2, 1.0m);
            var cof = Math.Round(cofRepo.GetTotalCofByPrefix("ITR"), 2);

            LogInfo($"UpsertToday(ITR): code={itrCode}, HI={(int)Math.Truncate(hi)}, CoF={cof}, PoF={pof}");
            riskRepo.UpsertToday(itrCode, (int)Math.Truncate(hi), cof, pof);
        }

        // 최신 RM을 오늘자 날짜로 복사 (최신행 없으면 return)
        static void CopyLatestRiskToTodayIfNeeded(RiskmatrixRepository riskRepo, string code)
        {
            var latest = riskRepo.GetLatestRiskMatrixByCode(code);
            if (latest == null)
            {
                LogWarn($"최신 RM 없음 --> 복사 스킵: {code}");
                return;
            }

            int? hi = int.TryParse(latest.HI, out var parsed) ? parsed : (int?)null;
            decimal? cof = latest.Cof;
            decimal? pof = latest.Pof;

            LogInfo($"UpsertToday(4종): code={code}, HI={(hi?.ToString() ?? "null")}, CoF={(cof?.ToString() ?? "null")}, PoF={(pof?.ToString() ?? "null")}");
            riskRepo.UpsertToday(code, hi, cof, pof);
        }

        // 4종 전체 처리: 보통점검 + 최신 RM 존재 시 오늘자 Upsert
        static void CopyAllFourTypesLatestToTodayIfExists(
            RiskmatrixRepository riskRepo,
            VCBChkRepository vcbRepo,
            DCCBChkRepository dccbRepo,
            DCCABLEChkRepository dccableRepo,
            SUBMODULEChkRepository submoduleRepo)
        {
            var latestPoints = riskRepo.GetLatestRiskPoints();
            int total = 0, processed = 0, skippedNoChk = 0, skippedNoRM = 0;

            foreach (var r in latestPoints)
            {
                var code = r.Code ?? string.Empty;
                if (code.StartsWith("ITR")) continue; // ITR 제외, 4종만

                total++;

                if (!HasChk(code, vcbRepo, dccbRepo, dccableRepo, submoduleRepo))
                {
                    skippedNoChk++;
                    continue;
                }

                var latest = riskRepo.GetLatestRiskMatrixByCode(code);
                if (latest == null)
                {
                    skippedNoRM++;
                    continue;
                }

                CopyLatestRiskToTodayIfNeeded(riskRepo, code);
                processed++;
            }

            LogInfo($"4종장비: 장비={total}, 처리={processed}, 스킵(보통점검없음)={skippedNoChk}, 스킵(최신RM없음)={skippedNoRM}");
            LogInfo($"-------------------------------------------------------------------------------------------------");

        }
    }
}
