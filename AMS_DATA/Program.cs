using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using AMS_MVC.Models;
using AMS_MVC.Repositories;
using AMS_MVC.Services;
using NodaTime.Extensions;

namespace AMS_DATA
{
    static class InfluxSignalMapper
    {
        public static readonly IReadOnlyDictionary<string, string> Map = new Dictionary<string, string>
        {
            //["YHLU01/SPDC1.ST.OpCnt."] = nameof(VCBChk.CHK_OperationCount),
            //["YHLU01/SPDC1.ST.DscnCnt."] = nameof(VCBChk.CHK_ShortCircuitCount),
            //["YHLU01/SPDC1.SP.EvtAmpTrhe2."] = nameof(VCBChk.CHK_PdPatternValue),
            //["YHLU01/STMP1.MV.Tmp."] = nameof(VCBChk.CHK_HotSpot),

            //["YHLU01/SPDC1.ST.MoDevComF."] = nameof(VCBChk.CHK_HotSpot),
            //["YHLU01/SPDC1.ST.MoDevFlt."] = nameof(VCBChk.CHK_HotSpot),
            //["YHLU01/SPDC1.MV.AppPaDsch."] = nameof(VCBChk.CHK_HotSpot),
            //["YHLU01/SPDC1.SP.EvtAmpTrhe2."]= nameof(VCBChk.CHK_HotSpot),

            //["YHLU01/SPDC1.ST.PaDschAlm."] = nameof(VCBChk.CHK_HotSpot),
            //["YHLU01/SPDC1.ST.EvtLvlSt."] = nameof(VCBChk.CHK_HotSpot),
            //["YHLU02/ITSTMP1.MX.Tmp."] = nameof(VCBChk.CHK_HotSpot),
            //["YHLU03/ITSTMP1.MX.Tmp."] = nameof(VCBChk.CHK_HotSpot),

            //["YHLU04/SCBR1.ST.EvtTransF."] = nameof(VCBChk.CHK_HotSpot),
            //["YHLU04/WTSTMP1.MX.Tmp."] = nameof(VCBChk.CHK_HotSpot),
            //["YHLU06/ZSAR1.ST.OpCnt."] = nameof(VCBChk.CHK_HotSpot),
            //["YHLU06/ZSAR1.MX.LeakA."] = nameof(VCBChk.CHK_HotSpot),

            ["KEPCOALM/CINGGIO1$ST$Ind01$q"] = nameof(VCBChk.CHK_OperationCount),
            ["KEPCOALM/CINGGIO1$ST$Ind02$q"] = nameof(VCBChk.CHK_ShortCircuitCount),
            ["KEPCOALM/CINGGIO1$ST$Ind03$q"] = nameof(VCBChk.CHK_PdPatternValue),
            ["KEPCOALM/CINGGIO1$ST$Ind04$q"] = nameof(VCBChk.CHK_HotSpot)
        };
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            // 0) 최신 VCB 코드 조회
            var basicRepo = new VCBBasicInfoRepository();
            var latestCode = basicRepo.GetLatestVCBCode();
            var basicInfo = basicRepo.GetVCBBasicInfoByCode(latestCode);


            Console.WriteLine($"▶ 처리할 VCB_CODE: {basicInfo.VCB_Code}, Serial: {basicInfo.Serial_No}\n");

            // InfluxDB 설정
            const string url = "http://192.168.0.24:8086";
            const string token = "cOPC3HVD8zxxSWUs2go2zWaVx7NAVEjVoX3cCAKzLc_QZJeUFoCJxvYjS8dKynP5s37jsYsfjA0baQLFKpE64Q==";
            const string org = "mvdc";
            const string bucket = "AMS";

            var client = InfluxDBClientFactory.Create(url, token);
            var chkRepo = new VCBChkRepository();
            var riskRepo = new RiskmatrixRepository();
            var cofRepo = new CoFRepository();

            // 기존 VCB_CHK 모두 로드
            chkRepo.GetVCBChkByVCBCode(basicInfo.VCB_Code, out var existing);

            // 오늘 날짜 레코드 찾기
            DateTime today = DateTime.Today;
            VCBChk todayRecord = existing.FirstOrDefault(x => x.CHK_Tbl_GetDate.Date == today);
            decimal? lastHi = todayRecord?.FoldingFunction;

            while (true)
            {
                // 1) InfluxDB에서 실시간 신호 읽기 + **디버그 출력**
                var signals = await FetchSignals(client, org, bucket);

                // influxdb에서 데이터 가져오는 개수가 0이면 연결 오류라 판단하여 업데이트 건너뜀
                   if (signals == null || signals.Count == 0)
                     {
                    Console.WriteLine("[WARN] InfluxDB에서 데이터를 가져오지 못했습니다. 업데이트를 건너뜁니다.");
                    await Task.Delay(TimeSpan.FromSeconds(10));
                      continue;
                     }

                // 2) 오늘 레코드가 없으면 새로 복제 후 INSERT
                if (todayRecord == null)
                {
                    var template = existing.OrderBy(x => x.Tbl_Idx).LastOrDefault();
                    todayRecord = template != null
                        ? CloneRecord(template)
                        : new VCBChk();

                    todayRecord.VCB_Code = basicInfo.VCB_Code;
                    todayRecord.CHK_Gongsa_Name = "예방진단 데이터";

                    FillModelFromSignals(todayRecord, signals);

                    var (hi, pof) = new VCBChkScoreCalculator()
                                       .CalculateHiPof(todayRecord, 1.0m);
                    todayRecord.FoldingFunction = (int)Math.Truncate(hi);

                    var cRes = chkRepo.CreateVCBChkRepo(todayRecord);
                    Console.WriteLine($"[CREATED] Success={cRes.IsSuccess}, Msg={cRes.Message}\n");

                    chkRepo.GetVCBChkByVCBCode(basicInfo.VCB_Code, out existing);
                    todayRecord = existing.OrderBy(x => x.Tbl_Idx).Last();
                    lastHi = todayRecord.FoldingFunction;
                }
                else
                {
                    FillModelFromSignals(todayRecord, signals);

                    var scoreList = new int[]
                    {
                        (int)todayRecord.CHK_ContactWearPercent,
                        (int)todayRecord.CHK_VacuumLeakCurrent,
                        (int)todayRecord.CHK_ContactResistance,
                        (int)todayRecord.CHK_InsulationResistance,
                        (int)todayRecord.CHK_HotSpot,
                        (int)todayRecord.CHK_PdPatternValue,
                        (int)todayRecord.CHK_MotorCurrent,
                        (int)todayRecord.CHK_AccumShortCircuitCurrent,
                        (int)todayRecord.CHK_ShortCircuitCount,
                        (int)todayRecord.CHK_OperationCount,
                        (int)todayRecord.CHK_OpenCloseTime,         
                        (int)todayRecord.CHK_VisualCheck
                    };

                    Console.WriteLine($"[DEBUG] Algorithm values: {string.Join(",", scoreList)}");

                    var (hi, pof) = new VCBChkScoreCalculator()
                                       .CalculateHiPof(todayRecord, 1.0m);
                    todayRecord.FoldingFunction = (int)Math.Truncate(hi);

                    Console.WriteLine($"[CALC] HI={hi:F2}, PoF={pof:F2}%, FoldingFunction={todayRecord.FoldingFunction}");

                    // 1) 업데이트 실행
                    var uRes = chkRepo.UpdateVCBChkInfoRepo(todayRecord);

                    decimal cofDec = Math.Round(cofRepo.GetTotalCofByPrefix("VCB"), 2);

                    var hiInt = (int)Math.Truncate(hi);
                    var pofDec = pof;

                    var rmRes = riskRepo.UpdateRiskMatrixHI(
                        basicInfo.VCB_Code,
                        hiInt,
                        cofDec,
                        pofDec
                    );
                    Console.WriteLine($"[RISKMATRIX] Success={rmRes.IsSuccess}, Msg={rmRes.Message}");

                    // 2) 바로 재조회
                    chkRepo.GetVCBChkDetailByVCBCode(basicInfo.VCB_Code, todayRecord.Tbl_Idx.ToString(), out var verifyList);
                    if (verifyList.Any())
                    {
                        var v = verifyList.First();
                        //Console.WriteLine($"[VERIFY] DB → CHK_OperationCount={v.CHK_OperationCount}, FOLDINGFUNCTION={v.FoldingFunction}");
                    }
                    else
                    {
                        Console.WriteLine("[VERIFY] 재조회 결과가 없습니다.");
                    }

                    lastHi = todayRecord.FoldingFunction;
                }

                // 날짜가 바뀌면 오늘 레코드 리셋
                if (DateTime.Today > today)
                {
                    today = DateTime.Today;
                    todayRecord = null;
                }

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }

        static async Task<Dictionary<string, float>> FetchSignals(
            InfluxDBClient client, string org, string bucket)
        {
            var flux = $@"
            from(bucket: ""{bucket}"")
                |> range(start: -10s)
                |> filter(fn: (r) => r._field == ""value"")
                |> last()
            ";
            try
            {
                var tables = await client.GetQueryApi().QueryAsync(flux, org);

                Console.WriteLine("==================== InfluxDB 데이터 ================================\n");
                foreach (var rec in tables.SelectMany(t => t.Records))
                {
                    var timeUtc = rec.GetTime()?.ToDateTimeUtc() ?? DateTime.MinValue;
                    var addr = rec.Values.ContainsKey("ADDR") ? rec.Values["ADDR"] : "<no-addr>";
                    Console.WriteLine($"time={timeUtc:o}, ADDR={addr}, value={rec.GetValue()}");
                }
                Console.WriteLine("=====================================================================\n");

                // build the dictionary
                var dict = new Dictionary<string, float>();
                foreach (var rec in tables.SelectMany(t => t.Records))
                {
                   // 1) 태그 addr 을 키로 사용
                    if (!rec.Values.TryGetValue("ADDR", out var addrObj))
                       continue;
                    var addrKey = addrObj.ToString();
                   // 2) 필드 value 를 float 로 파싱
                    if (float.TryParse(rec.GetValue().ToString(), out var v))
                        dict[addrKey] = v;
                }
                return dict;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] FetchSignals: {ex.Message}");
                return new Dictionary<string, float>();
            }
        }



        static VCBChk CloneRecord(VCBChk src)
        {
            var dst = new VCBChk();
            foreach (var p in typeof(VCBChk)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && p.Name != nameof(VCBChk.Tbl_Idx)))
            {
                p.SetValue(dst, p.GetValue(src));
            }
            return dst;
        }

        static void FillModelFromSignals(VCBChk model, Dictionary<string, float> signals)
        {
            Console.WriteLine("=== FillModelFromSignals 시작!!! ======================\n");
            foreach (var kv in signals)
            {
                if (InfluxSignalMapper.Map.TryGetValue(kv.Key, out var propName))
                {
                    var prop = typeof(VCBChk).GetProperty(propName);
                    if (prop != null)
                        prop.SetValue(model, kv.Value);
                    Console.WriteLine($"[MAPPED] ADDR=\"{kv.Key}\" → {propName} = {kv.Value}");
                }
                else
                {
                    Console.WriteLine($"[SKIPPED] ADDR=\"{kv.Key}\" (맵핑정보 없음)");
                }
            }
            Console.WriteLine("=== FillModelFromSignals 종료!!! =====================\n");
        }
    }
}