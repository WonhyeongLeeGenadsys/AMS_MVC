using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AMS_DATA;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;

using NodaTime.Extensions;
using Web.Common;

namespace AMS_DATA
{
    static class InfluxSignalMapper
    {
        public static readonly IReadOnlyDictionary<string, string> VCB = new Dictionary<string, string>
        {
        };

        public static readonly IReadOnlyDictionary<string, string> ITR1 = new Dictionary<string, string>
        {
        };

        public static readonly IReadOnlyDictionary<string, string> ITR2 = new Dictionary<string, string>
        {
            ["YHLU07/SPDC1$ST$PaDschAlm$stVal"] = nameof(ITRChk2.CHK2_PD),
        };
    }        
}



    class Program
    {
        static async Task Main(string[] args)
        {
            // 0) 최신 코드 조회
            var basicRepo = new VCBBasicInfoRepository();
            var ITRBasicRepo = new ITRBasicInfoRepository();

            var latestCode = basicRepo.GetLatestVCBCode();
            var ITRLatestCode = ITRBasicRepo.GetLatestITRCode();

            var basicInfo = basicRepo.GetVCBBasicInfoByCode(latestCode);
            var ITRbasicInfo = ITRBasicRepo.GetITRBasicInfoByITRCode(ITRLatestCode);

            Console.WriteLine($"▶ 처리할 VCB_CODE: {basicInfo.VCB_Code}, Serial: {basicInfo.Serial_No}\n");
            Console.WriteLine($"▶ 처리할 ITR_CODE: {ITRbasicInfo.ITR_Code}, Serial: {ITRbasicInfo.Serial_No}\n");

            // InfluxDB 설정
            const string url = "http://192.168.0.24:8086";
            //const string url = "http://127.0.0.1:8086";
            const string token = "cOPC3HVD8zxxSWUs2go2zWaVx7NAVEjVoX3cCAKzLc_QZJeUFoCJxvYjS8dKynP5s37jsYsfjA0baQLFKpE64Q==";
            const string org = "mvdc";
            const string bucket = "AMS";

            var client = InfluxDBClientFactory.Create(url, token);

            var chkRepo = new VCBChkRepository();
            var itr1Repo = new ITRChk1Repository();
            var itr2Repo = new ITRChk2Repository();

            var riskRepo = new RiskmatrixRepository();
            var cofRepo = new CoFRepository();

            var itrCalc = new ITRChkScoreCalculator();

            // 기존 CHK 모두 로드
            chkRepo.GetLatestVCBChkByVCBCode(basicInfo.VCB_Code, out var vcbExisting);
            itr1Repo.GetLatestITRChk1ByITRCode(ITRbasicInfo.ITR_Code, out var itr1Existing);
            itr2Repo.GetLatestITRChk2ByITRCode(ITRbasicInfo.ITR_Code, out var itr2Existing);

            // 오늘 레코드 찾기
            DateTime today = DateTime.Today;

            VCBChk todayVCB = vcbExisting?.FirstOrDefault(x => x.CHK_Tbl_GetDate.Date == today);
            ITRChk1 todayITR1 = itr1Existing?.FirstOrDefault(x => x.CHK1_Tbl_GetDate.Date == today);
            ITRChk2 todayITR2 = itr2Existing?.FirstOrDefault(x => x.CHK2_Tbl_GetDate.Date == today);

            while (true)
            {
                // 1) Influx 실시간 신호
                var signals = await FetchSignals(client, org, bucket);
                if (signals == null || signals.Count == 0)
                {
                    Console.WriteLine("[WARN] InfluxDB에서 데이터를 가져오지 못했습니다. 업데이트를 건너뜁니다.");
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    continue;
                }

                // =========================
                // VCB
                // =========================
                if (todayVCB == null)
                {
                    var template = vcbExisting?.OrderBy(x => x.Tbl_Idx).LastOrDefault();
                    todayVCB = template != null
                        ? CloneRecordVCBChk(template)
                        : new VCBChk { CHK_Tbl_GetDate = DateTime.Now }; // 신규 생성 시에도 지금 시간

                    todayVCB.VCB_Code = basicInfo.VCB_Code;
                    todayVCB.CHK_Gongsa_Name = "예방진단 데이터";

                    FillModelFromSignalsVCBChk(todayVCB, signals);

                    var (hi, pof) = new VCBChkScoreCalculator().CalculateHiPof(todayVCB, 1.0m);
                    todayVCB.FoldingFunction = (int)Math.Truncate(hi);

                    var cRes = chkRepo.CreateVCBChkRepo(todayVCB);
                    Console.WriteLine($"[CREATED VCB] Success={cRes.IsSuccess}, Msg={cRes.Message}\n");

                    // 생성 후 예전 목록 재사용 금지 → 재조회
                    chkRepo.GetLatestVCBChkByVCBCode(basicInfo.VCB_Code, out vcbExisting);
                    todayVCB = vcbExisting?.OrderBy(x => x.Tbl_Idx).LastOrDefault();
                }
                else
                {
                    FillModelFromSignalsVCBChk(todayVCB, signals);

                    var (hi, pof) = new VCBChkScoreCalculator().CalculateHiPof(todayVCB, 1.0m);
                    todayVCB.FoldingFunction = (int)Math.Truncate(hi);

                    var uRes = chkRepo.UpdateVCBChkInfoRepo(todayVCB);
                    Console.WriteLine($"[UPDATED VCB] Success={uRes.IsSuccess}, Msg={uRes.Message}");

                    var cof = Math.Round(cofRepo.GetTotalCofByPrefix("VCB"), 2);
                    var rm = riskRepo.UpdateRiskMatrixHI(basicInfo.VCB_Code, (int)Math.Truncate(hi), cof, pof);
                    Console.WriteLine($"[RISKMATRIX VCB] Success={rm.IsSuccess}, Msg={rm.Message}");
                }

                // =========================
                // ITR 보통점검 (ITRChk1)
                // =========================
                if (todayITR1 == null)
                {
                    var template1 = itr1Existing?.OrderBy(x => x.Tbl_Idx).LastOrDefault();
                    todayITR1 = template1 != null
                        ? CloneRecordITRChk1(template1)
                        : new ITRChk1 { CHK1_Tbl_GetDate = DateTime.Now };

                    todayITR1.ITR_Code = ITRbasicInfo.ITR_Code;
                    todayITR1.CHK1_Gongsa_Name = "예방진단 데이터";

                    FillModelFromSignalsITRChk1(todayITR1, signals);

                    var (hi1, pof1) = itrCalc.CalculateHiPof(todayITR1, 1.00m);
                    todayITR1.FoldingFunction = (int)Math.Truncate(hi1);

                    var c1 = itr1Repo.CreateITRChk1InfoRepo(todayITR1);
                    Console.WriteLine($"[CREATED ITR1] Success={c1.IsSuccess}, Msg={c1.Message}");

                    // 재조회
                    itr1Repo.GetLatestITRChk1ByITRCode(ITRbasicInfo.ITR_Code, out itr1Existing);
                    todayITR1 = itr1Existing?.OrderBy(x => x.Tbl_Idx).LastOrDefault();

                    var cof1 = Math.Round(cofRepo.GetTotalCofByPrefix("ITR"), 2);
                    var cof1Change = cof1 * (pof1 / 100m);

                    var rm1 = riskRepo.UpdateRiskMatrixHI(ITRbasicInfo.ITR_Code, (int)Math.Truncate(hi1), cof1Change, pof1);
                    Console.WriteLine($"[RISKMATRIX ITR1] Success={rm1.IsSuccess}, Msg={rm1.Message}");
                }
                else
                {
                    FillModelFromSignalsITRChk1(todayITR1, signals);

                    var (hi1, pof1) = itrCalc.CalculateHiPof(todayITR1, 1.00m);
                    todayITR1.FoldingFunction = (int)Math.Truncate(hi1);

                    var u1 = itr1Repo.UpdateITRChk1InfoRepo(todayITR1);
                    Console.WriteLine($"[UPDATED ITR1] Success={u1.IsSuccess}, Msg={u1.Message}");

                    var cof1 = Math.Round(cofRepo.GetTotalCofByPrefix("ITR"), 2);
                    var cof1Change = cof1 * (pof1 / 100m);

                    var rm1 = riskRepo.UpdateRiskMatrixHI(ITRbasicInfo.ITR_Code, (int)Math.Truncate(hi1), cof1, pof1);
                    Console.WriteLine($"[RISKMATRIX ITR1] Success={rm1.IsSuccess}, Msg={rm1.Message}");
                }

                // =========================
                // ITR 정밀점검 (ITRChk2)
                // =========================
                if (todayITR2 == null)
                {
                    var template2 = itr2Existing?.OrderBy(x => x.Tbl_Idx).LastOrDefault();
                    todayITR2 = template2 != null
                        ? CloneRecordITRChk2(template2)
                        : new ITRChk2 { CHK2_Tbl_GetDate = DateTime.Now };

                    todayITR2.ITR_Code = ITRbasicInfo.ITR_Code;
                    todayITR2.CHK2_Gongsa_Name = "예방진단 데이터";

                    FillModelFromSignalsITRChk2(todayITR2, signals);

                    var (hi2, pof2) = itrCalc.CalculateHiPof(todayITR2, 1.00m);
                    todayITR2.FoldingFunction = (int)Math.Truncate(hi2);

                    var c2 = itr2Repo.CreateITRChk2InfoRepo(todayITR2);
                    Console.WriteLine($"[CREATED ITR2] Success={c2.IsSuccess}, Msg={c2.Message}");

                    // 재조회
                    itr2Repo.GetLatestITRChk2ByITRCode(ITRbasicInfo.ITR_Code, out itr2Existing);
                    todayITR2 = itr2Existing?.OrderBy(x => x.Tbl_Idx).LastOrDefault();

                    var cof2 = Math.Round(cofRepo.GetTotalCofByPrefix("ITR"), 2);
                    var cof2Change = cof2 * (pof2 / 100m);

                    var rm2 = riskRepo.UpdateRiskMatrixHI(ITRbasicInfo.ITR_Code, (int)Math.Truncate(hi2), cof2Change, pof2);
                    Console.WriteLine($"[RISKMATRIX ITR2] Success={rm2.IsSuccess}, Msg={rm2.Message}");
                }
                else
                {
                    FillModelFromSignalsITRChk2(todayITR2, signals);

                    var (hi2, pof2) = itrCalc.CalculateHiPof(todayITR2, 1.00m);
                    todayITR2.FoldingFunction = (int)Math.Truncate(hi2);

                    var u2 = itr2Repo.UpdateITRChk2InfoRepo(todayITR2);
                    Console.WriteLine($"[UPDATED ITR2] Success={u2.IsSuccess}, Msg={u2.Message}");

                    var cof2 = Math.Round(cofRepo.GetTotalCofByPrefix("ITR"), 2);
                    var cof2Change = cof2 * (pof2 / 100m);

                    var rm2 = riskRepo.UpdateRiskMatrixHI(ITRbasicInfo.ITR_Code, (int)Math.Truncate(hi2), cof2Change, pof2);
                    Console.WriteLine($"[RISKMATRIX ITR2] Success={rm2.IsSuccess}, Msg={rm2.Message}");
                }

                // 날짜가 바뀌면 오늘 레코드 리셋
                if (DateTime.Today > today)
                {
                    today = DateTime.Today;
                    todayVCB = null;
                    todayITR1 = null;
                    todayITR2 = null;
                }

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }

        // -------------------- InfluxDB 연동--------------------
        static async Task<Dictionary<string, float>> FetchSignals(InfluxDBClient client, string org, string bucket)
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

                var dict = new Dictionary<string, float>();
                foreach (var rec in tables.SelectMany(t => t.Records))
                {
                    if (!rec.Values.TryGetValue("ADDR", out var addrObj))
                        continue;

                    var addrKey = addrObj.ToString();
                    if (float.TryParse(rec.GetValue()?.ToString(), out var v))
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

        // -------------------- DB 복사하기! --------------------
        static VCBChk CloneRecordVCBChk(VCBChk src)
        {
            var dst = new VCBChk();
            foreach (var p in typeof(VCBChk).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .Where(p => p.CanWrite
                              && p.Name != nameof(VCBChk.Tbl_Idx)
                              && p.Name != nameof(VCBChk.CHK_Tbl_GetDate))) // 날짜는 복사 안 함
                p.SetValue(dst, p.GetValue(src));

            dst.CHK_Tbl_GetDate = DateTime.Now; // 지금 시간
            return dst;
        }

        static ITRChk1 CloneRecordITRChk1(ITRChk1 src)
        {
            var dst = new ITRChk1();
            foreach (var p in typeof(ITRChk1).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .Where(p => p.CanWrite
                              && p.Name != nameof(ITRChk1.Tbl_Idx)
                              && p.Name != nameof(ITRChk1.CHK1_Tbl_GetDate)))
                p.SetValue(dst, p.GetValue(src));

            dst.CHK1_Tbl_GetDate = DateTime.Now; // 지금 시간
            return dst;
        }

        static ITRChk2 CloneRecordITRChk2(ITRChk2 src)
        {
            var dst = new ITRChk2();
            foreach (var p in typeof(ITRChk2).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .Where(p => p.CanWrite
                              && p.Name != nameof(ITRChk2.Tbl_Idx)
                              && p.Name != nameof(ITRChk2.CHK2_Tbl_GetDate)))
                p.SetValue(dst, p.GetValue(src));

            dst.CHK2_Tbl_GetDate = DateTime.Now; // 지금 시간
            return dst;
        }

        // -------------------- 데이터 맵핑하기!!! --------------------
        static void FillModelFromSignalsVCBChk(VCBChk model, Dictionary<string, float> signals)
        {
            Console.WriteLine("=== FillModelFromSignals(VCB) ===========================\n");
            foreach (var kv in signals)
            {
                if (!InfluxSignalMapper.VCB.TryGetValue(kv.Key, out var propName))
                {
                    Console.WriteLine($"[SKIPPED][VCB] {kv.Key} (맵핑정보 없음)");
                    continue;
                }
                var prop = typeof(VCBChk).GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                if (prop == null || !prop.CanWrite)
                {
                    Console.WriteLine($"[SKIPPED][VCB] 세터 없음: {propName}");
                    continue;
                }
                SetValue(prop, model, kv.Value);
                Console.WriteLine($"[MAPPED][VCB] {kv.Key} → {propName} = {kv.Value}");
            }
            Console.WriteLine("=========================================================\n");
        }

        static void FillModelFromSignalsITRChk1(ITRChk1 model, Dictionary<string, float> signals)
        {
            Console.WriteLine("=== FillModelFromSignals(ITR1) ==========================\n");
            foreach (var kv in signals)
            {
                if (!InfluxSignalMapper.ITR1.TryGetValue(kv.Key, out var propName))
                {
                    Console.WriteLine($"[SKIPPED][ITR1] {kv.Key} (맵핑정보 없음)");
                    continue;
                }
                var prop = typeof(ITRChk1).GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                if (prop == null || !prop.CanWrite)
                {
                    Console.WriteLine($"[SKIPPED][ITR1] 세터 없음: {propName}");
                    continue;
                }
                SetValue(prop, model, kv.Value);
                Console.WriteLine($"[MAPPED][ITR1] {kv.Key} → {propName} = {kv.Value}");
            }
            Console.WriteLine("=========================================================\n");
        }

        static void FillModelFromSignalsITRChk2(ITRChk2 model, Dictionary<string, float> signals)
        {
            Console.WriteLine("=== FillModelFromSignals(ITR2) ==========================\n");
            foreach (var kv in signals)
            {
                if (!InfluxSignalMapper.ITR2.TryGetValue(kv.Key, out var propName))
                {
                    Console.WriteLine($"[SKIPPED][ITR2] {kv.Key} (맵핑정보 없음)");
                    continue;
                }
                var prop = typeof(ITRChk2).GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                if (prop == null || !prop.CanWrite)
                {
                    Console.WriteLine($"[SKIPPED][ITR2] 세터 없음: {propName}");
                    continue;
                }

                if (kv.Key == "YHLU07/SPDC1$ST$PaDschAlm$stVal" && propName == nameof(ITRChk2.CHK2_PD))
                {
                    int score = (kv.Value == 0f) ? 1 : 5;
                    prop.SetValue(model, score);
                    Console.WriteLine($"[MAPPED][ITR2:PD] {kv.Key} → {propName} = {score} (raw={kv.Value})");
                }
                else
                {
                    SetValue(prop, model, kv.Value);
                    Console.WriteLine($"[MAPPED][ITR2] {kv.Key} → {propName} = {kv.Value}");
                }
            }
            Console.WriteLine("=========================================================\n");
        }

        // 받아들이는 float 타입을 --> 숫자 및 문자열로 변환하기!
        static void SetValue(PropertyInfo prop, object target, float raw)
        {
            var t = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            object val =
                t == typeof(int) ? Convert.ToInt32(raw) :
                t == typeof(long) ? Convert.ToInt64(raw) :
                t == typeof(decimal) ? (decimal)raw :
                t == typeof(double) ? (double)raw :
                t == typeof(float) ? raw :
                t == typeof(string) ? raw.ToString() :
                Convert.ChangeType(raw, t);

            prop.SetValue(target, val);
        }
    }

