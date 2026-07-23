using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
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
                // AC 차단기
                ["YHLU07/SPDC1$ST$PaDschAlm$stVal"] = "VCB001",
                ["YHLU07/SPDC2$ST$PaDschAlm$stVal"] = "VCB001",

                // AC 변압기
                ["YHLU06/SPDC1$ST$PaDschAlm$stVal"] = "ITR001",
                ["YHLU06/SPDC2$ST$PaDschAlm$stVal"] = "ITR001",
                ["YHLU06/SPDC3$ST$PaDschAlm$stVal"] = "ITR002",
                ["YHLU06/SPDC4$ST$PaDschAlm$stVal"] = "ITR002",

                // DC 차단기
                ["YHLU01/SPDC1$ST$PaDschAlm$stVal"] = "DCCB001",
                ["YHLU01/SPDC2$ST$PaDschAlm$stVal"] = "DCCB001",

                // DC 접속재(DC Cable)
                ["YHLU04/SPDC1$ST$PaDschAlm$stVal"] = "DCCABLE001",
                ["YHLU04/SPDC2$ST$PaDschAlm$stVal"] = "DCCABLE001",

                // DC 변압기(Submodule)
                ["YHLU02/SPDC1$ST$PaDschAlm$stVal"] = "SUBMODULE001",
                ["YHLU02/SPDC2$ST$PaDschAlm$stVal"] = "SUBMODULE001",

                // 온도 센서
                ["YHLU07/WTSTMP1$MX$Tmp$mag$f"] = "VCB001",
                ["YHLU07/WTSTMP2$MX$Tmp$mag$f"] = "VCB001",
                ["YHLU06/ITSTMP1$MX$Tmp$mag$f"] = "ITR001",
                ["YHLU06/ITSTMP2$MX$Tmp$mag$f"] = "ITR001",
                ["YHLU06/ITSTMP3$MX$Tmp$mag$f"] = "ITR002",
                ["YHLU06/ITSTMP4$MX$Tmp$mag$f"] = "ITR002",
            };

        // 온도
        public static readonly HashSet<string> TEMPERATURE_ADDRS = new HashSet<string>(StringComparer.Ordinal)
            {
                "YHLU07/WTSTMP1$MX$Tmp$mag$f",
                "YHLU07/WTSTMP2$MX$Tmp$mag$f",

                "YHLU06/ITSTMP1$MX$Tmp$mag$f",
                "YHLU06/ITSTMP2$MX$Tmp$mag$f",

                "YHLU06/ITSTMP3$MX$Tmp$mag$f",
                "YHLU06/ITSTMP4$MX$Tmp$mag$f",
            };
    }

    enum SignalKind
    {
        PartialDischarge,
        Temperature
    }

    class SignalHit
    {
        public string Addr;
        public string Code;
        public float Value;
        public SignalKind Kind;
    }

    class Program
    {
        private static readonly object ConsoleLock = new object();

        static void WriteLog(string level, ConsoleColor color, string msg)
        {
            lock (ConsoleLock)
            {
                var originalColor = Console.ForegroundColor;
                if (!Console.IsOutputRedirected)
                    Console.ForegroundColor = color;

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{level,-7}] {msg}");

                if (!Console.IsOutputRedirected)
                    Console.ForegroundColor = originalColor;
            }
        }

        static void LogInfo(string msg) => WriteLog("INFO", ConsoleColor.Gray, msg);
        static void LogData(string msg) => WriteLog("SENSOR", ConsoleColor.Cyan, msg);
        static void LogSuccess(string msg) => WriteLog("SUCCESS", ConsoleColor.Green, msg);
        static void LogWarn(string msg) => WriteLog("WARN", ConsoleColor.Yellow, msg);
        static void LogError(string msg, Exception ex = null)
        {
            WriteLog("ERROR", ConsoleColor.Red, msg);
            if (ex != null)
                WriteLog("ERROR", ConsoleColor.DarkRed, ex.ToString());
        }

        static void LogSection(string title)
        {
            lock (ConsoleLock)
            {
                var originalColor = Console.ForegroundColor;
                if (!Console.IsOutputRedirected)
                    Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine();
                Console.WriteLine(new string('=', 110));
                Console.WriteLine($"  {title}");
                Console.WriteLine(new string('=', 110));

                if (!Console.IsOutputRedirected)
                    Console.ForegroundColor = originalColor;
            }
        }

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            try { Console.Title = "AMS InfluxDB 센서 수집 및 HI 계산"; } catch { }

            var vcbChkRepo = new VCBChkRepository();
            var itr1ChkRepo = new ITRChk1Repository();
            var itr2ChkRepo = new ITRChk2Repository();
            var dccbChkRepo = new DCCBChkRepository();
            var dccableChkRepo = new DCCABLEChkRepository();
            var submoduleChkRepo = new SUBMODULEChkRepository();

            var riskRepo = new RiskmatrixRepository();
            var cofRepo = new CoFRepository();
            var vcbCalc = new VCBChkScoreCalculator();
            var itrCalc = new ITRChkScoreCalculator();
            var dccbCalc = new DCCBChkScoreCalculator();
            var dccableCalc = new DCCABLEChkScoreCalculator();

            string url = ConfigurationManager.AppSettings["InfluxUrl"];          
            string token = ConfigurationManager.AppSettings["InfluxToken"];      
            string org = ConfigurationManager.AppSettings["InfluxOrg"] ?? "Genadsys";
            string bucket = ConfigurationManager.AppSettings["InfluxBucket"] ?? "AMS";
            var client = InfluxDBClientFactory.Create(url, token);

            LogSection("AMS_DATA 시작");
            LogInfo($"Influx URL={url}, ORG={org}, BUCKET={bucket}");
            LogInfo($"주소 매핑={AddrCodeMap.ADDR_TO_CODE.Count}개, 수집 주기=10초, 조회 범위=최근 15초");

            // 자정 1회 처리 마커
            DateTime lastDailyProcessed = DateTime.MinValue.Date;

            while (true)
            {
                try
                {
                    LogSection($"Influx 수집 주기 시작 | {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    var hits = await FetchSignalsWithCodes(client, org, bucket);
                    LogInfo($"매핑·파싱 완료 신호: {hits.Count}개");

                    if (hits.Count == 0)
                        LogWarn("이번 주기에 처리할 매핑 신호가 없습니다.");

                    // 신호 들어온 code별 처리
                    foreach (var grp in hits.GroupBy(h => h.Code).OrderBy(g => g.Key))
                    {
                        var code = grp.Key;
                        var pdHits = grp.Where(h => h.Kind == SignalKind.PartialDischarge).ToList();
                        var temperatureHits = grp.Where(h => h.Kind == SignalKind.Temperature).ToList();

                        LogSection($"장비 처리 | {code}");
                        foreach (var hit in grp.OrderBy(h => h.Kind).ThenBy(h => h.Addr))
                        {
                            LogData($"{GetSignalKindText(hit.Kind),-8} | 값={FormatValue(hit.Value),8} | {hit.Addr}");
                        }

                        // 동일 장비의 PD 센서 중 하나라도 경보이면 해당 장비 경보로 처리한다.
                        float? pdValue = pdHits.Any() ? pdHits.Max(h => h.Value) : (float?)null;
                        int? pdGrade = pdValue.HasValue
                            ? GetPartialDischargeGrade(pdValue.Value)
                            : (int?)null;

                        if (pdGrade.HasValue)
                        {
                            LogInfo($"부분방전 집계 | 센서={pdHits.Count}개, 최대값={FormatValue(pdValue.Value)}, HI 입력등급={pdGrade.Value}");
                        }
                        else
                        {
                            LogInfo("부분방전 신호 없음 | 기존 점검값 유지");
                        }

                        var temperatureSensors = temperatureHits
                            .GroupBy(h => h.Addr)
                            .Select(g => new { Addr = g.Key, Value = g.Last().Value })
                            .OrderBy(x => x.Addr)
                            .ToList();

                        float? temperatureDifference = null;
                        int? temperatureGrade = null;

                        if (temperatureSensors.Count >= 2)
                        {
                            temperatureDifference = temperatureSensors.Max(x => x.Value) - temperatureSensors.Min(x => x.Value);
                            temperatureGrade = GetTemperatureGrade(temperatureDifference.Value);
                            var valuesText = string.Join(", ", temperatureSensors.Select(x => $"{x.Addr}={FormatValue(x.Value)}℃"));
                            LogInfo($"온도 편차 계산 | {valuesText} | 편차={FormatValue(temperatureDifference.Value)}℃, HI 입력등급={temperatureGrade.Value}");
                        }
                        else if (temperatureSensors.Count == 1)
                        {
                            LogWarn($"온도 센서 1개만 수집 | 주소={temperatureSensors[0].Addr}, 값={FormatValue(temperatureSensors[0].Value)}℃ | 편차 등급 계산 불가, 기존 점검값 유지");
                        }
                        else
                        {
                            LogInfo("온도 신호 없음 | 기존 점검값 유지");
                        }

                        if (code.StartsWith("ITR"))
                        {
                            // ITR: 신호 있을 때만 HI/PoF 계산하여 오늘자 RM Upsert (보통점검 이력이 존재한다면!)
                            if (HasChk(code, itr1ChkRepo, itr2ChkRepo))
                            {
                                UpdateItrRiskFromSensors(itr1ChkRepo, itr2ChkRepo, itrCalc, cofRepo, riskRepo, code, pdGrade, temperatureGrade);
                            }
                            else
                            {
                                LogWarn($"ITR 점검 데이터 없음 | 센서 HI 반영 스킵: {code}");
                            }
                        }
                        else
                        {
                            if (HasChk(code, vcbChkRepo, dccbChkRepo, dccableChkRepo, submoduleChkRepo))
                            {
                                if (code.StartsWith("VCB") && (pdGrade.HasValue || temperatureGrade.HasValue))
                                {
                                    UpdateVcbRiskFromSensors(vcbChkRepo, vcbCalc, cofRepo, riskRepo, code, pdGrade, temperatureGrade);
                                }
                                else if (code.StartsWith("DCCB") && pdGrade.HasValue)
                                {
                                    UpdateDccbRiskFromSensor(dccbChkRepo, dccbCalc, cofRepo, riskRepo, code, pdGrade.Value);
                                }
                                else if (code.StartsWith("DCCABLE") && pdGrade.HasValue)
                                {
                                    UpdateDccableRiskFromSensor(dccableChkRepo, dccableCalc, cofRepo, riskRepo, code, pdGrade.Value);
                                }
                                else
                                {
                                    if (code.StartsWith("SUBMODULE") && pdGrade.HasValue)
                                    {
                                        LogWarn("SUBMODULE 부분방전 신호 수집됨 | Health Index 기준표와 현재 모델에 PD 평가항목이 없어 HI에는 반영하지 않음");
                                    }

                                    LogInfo($"센서로 변경할 HI 평가항목 없음 | 최신 RiskMatrix 오늘자 복사: {code}");
                                    CopyLatestRiskToTodayIfNeeded(riskRepo, code);
                                }
                            }
                            else
                            {
                                LogWarn($"보통점검 데이터 없음 | 센서 HI 반영 스킵: {code}");
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

                    LogSuccess($"Influx 수집 주기 완료 | 처리 장비={hits.Select(h => h.Code).Distinct().Count()}대, 처리 신호={hits.Count}개");
                }
                catch (Exception ex)
                {
                    // 어떤 예외도 서비스 중단 없이 다음 틱에서 재시도
                    LogError("메인 루프 처리 중 예외 발생(루프는 계속 돈다).", ex);
                }

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }

        // Influx에서 신호를 읽고, 모든 수신값을 콘솔에 표시한 뒤 주소 --> (코드, 값)으로 매핑한다.
        static async Task<List<SignalHit>> FetchSignalsWithCodes(InfluxDBClient client, string org, string bucket)
        {
            var flux = $@"
            from(bucket: ""{bucket}"")
              |> range(start: -15s)
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
                    var rawValue = rec.GetValue();
                    var valueText = rawValue?.ToString() ?? "null";
                    var timeText = rec.Values.TryGetValue("_time", out var timeObj) && timeObj != null
                        ? timeObj.ToString()
                        : "시간없음";

                    if (!rec.Values.TryGetValue("ADDR", out var addrObj) || addrObj == null)
                    {
                        LogWarn($"Influx 원본 #{total:000} | ADDR 없음 | 값={valueText} | 시간={timeText}");
                        continue;
                    }

                    var addr = addrObj.ToString();
                    if (string.IsNullOrWhiteSpace(addr))
                    {
                        LogWarn($"Influx 원본 #{total:000} | ADDR 빈값 | 값={valueText} | 시간={timeText}");
                        continue;
                    }

                    if (!AddrCodeMap.ADDR_TO_CODE.TryGetValue(addr, out var code))
                    {
                        LogData($"Influx 원본 #{total:000} | 미매핑 | 값={valueText,-10} | {addr} | 시간={timeText}");
                        continue;
                    }
                    mapped++;

                    var valStr = rawValue?.ToString();
                    bool parsedValue = float.TryParse(
                        valStr,
                        NumberStyles.Float | NumberStyles.AllowThousands,
                        CultureInfo.InvariantCulture,
                        out var v);

                    if (!parsedValue)
                        parsedValue = float.TryParse(valStr, out v);

                    if (parsedValue)
                    {
                        var kind = AddrCodeMap.TEMPERATURE_ADDRS.Contains(addr)
                            ? SignalKind.Temperature
                            : SignalKind.PartialDischarge;
                        result.Add(new SignalHit { Addr = addr, Code = code, Value = v, Kind = kind });
                        parsed++;
                        LogData($"Influx 원본 #{total:000} | 매핑됨 | {code,-14} | {GetSignalKindText(kind),-8} | 값={FormatValue(v),8} | {addr} | 시간={timeText}");
                    }
                    else
                    {
                        LogWarn($"Influx 원본 #{total:000} | 숫자 변환 실패 | {code} | 값={valueText} | {addr} | 시간={timeText}");
                    }
                }
                LogInfo($"Flux 결과 요약 | 전체={total}개, 주소매핑={mapped}개, 숫자변환={parsed}개, 미매핑/제외={total - parsed}개");
            }
            catch (Exception ex)
            {
                LogError("FetchSignalsWithCodes 예외 발생.", ex);
            }

            return result;
        }

        static string GetSignalKindText(SignalKind kind)
        {
            return kind == SignalKind.Temperature ? "온도" : "부분방전";
        }

        static string FormatValue(float value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }

        // 현재 주소는 부분방전 크기값이 아니라 경보 상태값(stVal)이므로 정상=1등급, 경보=5등급으로 변환한다.
        static int GetPartialDischargeGrade(float pdAlarmValue)
        {
            return Math.Abs(pdAlarmValue) < 0.000001f ? 1 : 5;
        }

        // HI 모듈 기준: 유사 부위 온도 편차 <3℃=1등급, 3~15℃=2등급, >15℃=4등급
        static int GetTemperatureGrade(float temperatureDifference)
        {
            if (temperatureDifference < 3f) return 1;
            if (temperatureDifference <= 15f) return 2;
            return 4;
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

        // 센서값은 최신 점검 객체의 해당 평가항목에 계산용으로 반영한다.
        // 수동 점검 원본 행은 덮어쓰지 않고, 계산된 HI/PoF만 오늘자 RiskMatrix에 저장한다.
        static void UpdateItrRiskFromSensors(
            ITRChk1Repository itr1Repo,
            ITRChk2Repository itr2Repo,
            ITRChkScoreCalculator itrCalc,
            CoFRepository cofRepo,
            RiskmatrixRepository riskRepo,
            string itrCode,
            int? pdGrade,
            int? temperatureGrade)
        {
            itr1Repo.GetLatestITRChk1ByITRCode(itrCode, out var l1);
            itr2Repo.GetLatestITRChk2ByITRCode(itrCode, out var l2);

            var latest1 = l1?.OrderBy(x => x.Tbl_Idx).LastOrDefault();
            var latest2 = l2?.OrderBy(x => x.Tbl_Idx).LastOrDefault();
            if (latest1 == null && latest2 == null) return;

            var t1 = latest1 ?? new ITRChk1();
            var t2 = latest2 ?? new ITRChk2();
            int previousPdGrade = t2.CHK2_PD;
            int previousTemperatureGrade = t1.CHK1_Thermal_Temperature;

            if (pdGrade.HasValue)
                t2.CHK2_PD = pdGrade.Value;

            if (temperatureGrade.HasValue)
                t1.CHK1_Thermal_Temperature = temperatureGrade.Value;

            LogInfo($"ITR 센서 파라미터 적용 | 부분방전 {previousPdGrade} → {t2.CHK2_PD}, 열화상·온도 {previousTemperatureGrade} → {t1.CHK1_Thermal_Temperature}");

            var (hi, pof) = itrCalc.CalculateHiPofCombined(t1, t2, 1.0m);
            var cof = Math.Round(cofRepo.GetTotalCofByPrefix("ITR"), 2);

            UpsertCalculatedRisk(riskRepo, itrCode, "ITR", hi, cof, pof);
        }

        static void UpdateVcbRiskFromSensors(
            VCBChkRepository vcbRepo,
            VCBChkScoreCalculator vcbCalc,
            CoFRepository cofRepo,
            RiskmatrixRepository riskRepo,
            string vcbCode,
            int? pdGrade,
            int? temperatureGrade)
        {
            vcbRepo.GetLatestVCBChkByVCBCode(vcbCode, out var list);
            var latest = list?.OrderBy(x => x.Tbl_Idx).LastOrDefault();
            if (latest == null)
            {
                LogWarn($"VCB 보통점검 없음 | 센서 HI 반영 스킵: {vcbCode}");
                return;
            }

            int previousPdGrade = (int)latest.CHK_PdPatternValue;
            int previousTemperatureGrade = (int)latest.CHK_ThermalTemperature;

            if (pdGrade.HasValue)
                latest.CHK_PdPatternValue = pdGrade.Value;

            if (temperatureGrade.HasValue)
                latest.CHK_ThermalTemperature = temperatureGrade.Value;

            LogInfo($"VCB 센서 파라미터 적용 | PD 패턴·성장 {previousPdGrade} → {(int)latest.CHK_PdPatternValue}, 열화상·온도 {previousTemperatureGrade} → {(int)latest.CHK_ThermalTemperature}");

            var (hi, pof) = vcbCalc.CalculateHiPof(latest, 1.0m);
            var cof = Math.Round(cofRepo.GetTotalCofByPrefix("VCB"), 2);

            UpsertCalculatedRisk(riskRepo, vcbCode, "VCB", hi, cof, pof);
        }

        static void UpdateDccbRiskFromSensor(
            DCCBChkRepository dccbRepo,
            DCCBChkScoreCalculator dccbCalc,
            CoFRepository cofRepo,
            RiskmatrixRepository riskRepo,
            string dccbCode,
            int pdGrade)
        {
            dccbRepo.GetLatestDCCBChkByDCCBCode(dccbCode, out var list);
            var latest = list?.OrderBy(x => x.Tbl_Idx).LastOrDefault();
            if (latest == null)
            {
                LogWarn($"DCCB 보통점검 없음 | 센서 HI 반영 스킵: {dccbCode}");
                return;
            }

            int previousPdGrade = (int)latest.CHK_MainCircuit_PD;
            latest.CHK_MainCircuit_PD = pdGrade;
            LogInfo($"DCCB 센서 파라미터 적용 | 주회로 부분방전 {previousPdGrade} → {(int)latest.CHK_MainCircuit_PD}");

            var (hi, pof) = dccbCalc.CalculateHiPof(latest, 1.0m);
            var cof = Math.Round(cofRepo.GetTotalCofByPrefix("DCCB"), 2);

            UpsertCalculatedRisk(riskRepo, dccbCode, "DCCB", hi, cof, pof);
        }

        static void UpdateDccableRiskFromSensor(
            DCCABLEChkRepository dccableRepo,
            DCCABLEChkScoreCalculator dccableCalc,
            CoFRepository cofRepo,
            RiskmatrixRepository riskRepo,
            string dccableCode,
            int pdGrade)
        {
            dccableRepo.GetLatestDCCABLEChkByDCCABLECode(dccableCode, out var list);
            var latest = list?.OrderBy(x => x.Tbl_Idx).LastOrDefault();
            if (latest == null)
            {
                LogWarn($"DC Cable 보통점검 없음 | 센서 HI 반영 스킵: {dccableCode}");
                return;
            }

            int previousPdGrade = (int)latest.CHK_Partial_Discharge;
            latest.CHK_Partial_Discharge = pdGrade;
            LogInfo($"DC Cable 센서 파라미터 적용 | 부분방전(PD) {previousPdGrade} → {(int)latest.CHK_Partial_Discharge}");

            var (hi, pof) = dccableCalc.CalculateHiPof(latest, 1.0m);
            var cof = Math.Round(cofRepo.GetTotalCofByPrefix("DCCABLE"), 2);

            UpsertCalculatedRisk(riskRepo, dccableCode, "DC Cable", hi, cof, pof);
        }

        static void UpsertCalculatedRisk(
            RiskmatrixRepository riskRepo,
            string code,
            string equipmentName,
            decimal hi,
            decimal cof,
            decimal pof)
        {
            int newHi = (int)Math.Truncate(hi);
            var previous = riskRepo.GetLatestRiskMatrixByCode(code);
            string previousHi = previous?.HI ?? "없음";

            LogInfo($"{equipmentName} HI 계산 결과 | CODE={code} | HI {previousHi} → {newHi} | PoF={pof:F6}% | CoF={cof:F2}");

            try
            {
                riskRepo.UpsertToday(code, newHi, cof, pof);
                LogSuccess($"RiskMatrix 오늘자 반영 완료 | CODE={code}, HI={newHi}, PoF={pof:F6}%, CoF={cof:F2}");
            }
            catch (Exception ex)
            {
                LogError($"RiskMatrix 오늘자 반영 실패 | CODE={code}", ex);
            }
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
