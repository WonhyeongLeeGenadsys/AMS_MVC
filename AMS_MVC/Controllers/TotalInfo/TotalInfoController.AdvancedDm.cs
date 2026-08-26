using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Web.Common;

namespace AMS_MVC
{
    public partial class TotalInfoController
    {
        [HttpGet]
        public JsonResult GetDMAdvancedAnalysis(string prefix = "", double? budget = null)
        {
            try
            {
                var decisions = new DmDecisionService().GetDecisions(prefix);
                DmAdvancedAnalysisInfo result = new DmAdvancedAnalysisService()
                    .Analyze(decisions, budget);
                return Json(new { success = true, result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("TotalInfoController", "GetDMAdvancedAnalysis Error: " + ex.Message);
                return Json(new
                {
                    success = false,
                    error = "고급 의사결정 분석 중 오류가 발생했습니다.",
                    details = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public FileResult DownloadDMResult(string prefix = "")
        {
            var decisions = new DmDecisionService().GetDecisions(prefix);
            var builder = new StringBuilder();
            builder.AppendLine("순위,구분,설비코드,설비명,HI,PoF(%),CoF($),Risk($),BCR,NPV(원),ROI(%),RUL(년),TOPSIS,의사결정,권장시기");
            foreach (DmDecisionInfo item in decisions)
            {
                builder.Append(Csv(item.TopsisRank)).Append(',')
                    .Append(Csv(item.Sort)).Append(',')
                    .Append(Csv(item.Code)).Append(',')
                    .Append(Csv(item.ProductName)).Append(',')
                    .Append(Csv(item.HI)).Append(',')
                    .Append(Csv(item.PoFRatio * 100d)).Append(',')
                    .Append(Csv(item.CoF)).Append(',')
                    .Append(Csv(item.Risk)).Append(',')
                    .Append(Csv(item.Bcr)).Append(',')
                    .Append(Csv(item.NpvValue)).Append(',')
                    .Append(Csv(item.RoiPct)).Append(',')
                    .Append(Csv(item.RULYears)).Append(',')
                    .Append(Csv(item.TopsisScore)).Append(',')
                    .Append(Csv(item.Decision)).Append(',')
                    .Append(Csv(item.RecommendedAction))
                    .AppendLine();
            }

            byte[] preamble = Encoding.UTF8.GetPreamble();
            byte[] body = Encoding.UTF8.GetBytes(builder.ToString());
            byte[] contents = preamble.Concat(body).ToArray();
            string fileName = "MVDC_AMS_DM_Result_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
            return File(contents, "text/csv; charset=utf-8", fileName);
        }

        [HttpGet]
        public FileResult DownloadDMCimJsonLd(string prefix = "")
        {
            var decisions = new DmDecisionService().GetDecisions(prefix);
            var graph = new JArray
            {
                new JObject
                {
                    ["@id"] = "urn:mvdc-ams:model:v3.1.0",
                    ["@type"] = "md:FullModel",
                    ["md:Model.profile"] = "IEC61968-4 Asset + IEC61970-301/302 + ams: extension",
                    ["md:Model.version"] = "MVDC-AMS-v3.1.0"
                }
            };

            foreach (DmDecisionInfo item in decisions)
            {
                string assetId = "urn:mvdc-ams:asset:" + (item.Code ?? "UNKNOWN");
                string psrId = "urn:mvdc-ams:psr:" + (item.Code ?? "UNKNOWN");
                string psrClass = CimClass(item.AssetType);
                if (!string.IsNullOrEmpty(psrClass))
                {
                    graph.Add(new JObject
                    {
                        ["@id"] = psrId,
                        ["@type"] = "cim:" + psrClass,
                        ["cim:IdentifiedObject.mRID"] = item.Code,
                        ["cim:IdentifiedObject.name"] = item.ProductName
                    });
                }

                var asset = new JObject
                {
                    ["@id"] = assetId,
                    ["@type"] = "cim:Asset",
                    ["cim:Asset.type"] = item.AssetType,
                    ["ams:assetCode"] = item.Code,
                    ["ams:healthIndex"] = item.HI,
                    ["ams:probabilityOfFailure"] = item.PoFRatio,
                    ["ams:riskScore"] = item.Risk,
                    ["ams:cofFinancial"] = item.CofFinancial,
                    ["ams:cofReliability"] = item.CofReliability,
                    ["ams:cofSafety"] = item.CofSafety,
                    ["ams:cofEnvironmental"] = item.CofEnvironmental,
                    ["ams:remainingUsefulLifeYears"] = item.RULYears,
                    ["ams:decision"] = item.Decision,
                    ["ams:topsisRank"] = item.TopsisRank
                };
                if (!string.IsNullOrEmpty(psrClass))
                {
                    asset["cim:Asset.PowerSystemResources"] = new JObject { ["@id"] = psrId };
                }
                graph.Add(asset);
            }

            var document = new JObject
            {
                ["@context"] = new JObject
                {
                    ["cim"] = "http://iec.ch/TC57/CIM100#",
                    ["ams"] = "https://mvdc-ams.kepco.co.kr/ns/ams#",
                    ["md"] = "http://iec.ch/TC57/61970-552/ModelDescription/1#"
                },
                ["@graph"] = graph
            };
            return Utf8File(document.ToString(Formatting.Indented),
                "application/ld+json; charset=utf-8",
                "MVDC_AMS_CIM_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".jsonld");
        }

        [HttpGet]
        public FileResult DownloadISO55001Report(string prefix = "")
        {
            var rows = new DmDecisionService().GetDecisions(prefix);
            double[] ahp = AmsV31DecisionMath.CalculateAhpWeights();
            var builder = new StringBuilder();
            builder.AppendLine("# ISO 55001:2014 자산관리 정합 리포트")
                .AppendLine()
                .AppendLine("- 산출 기준: MVDC AMS v3.1.0")
                .AppendLine("- 산출 일시: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                .AppendLine("- 분석 자산: " + rows.Count + "대")
                .AppendLine("- AHP 일관성비율: " + AmsV31DecisionMath.CalculateConsistencyRatio(ahp).ToString("F4", CultureInfo.InvariantCulture))
                .AppendLine()
                .AppendLine("## 핵심 KPI")
                .AppendLine()
                .AppendLine("- 평균 HI: " + (rows.Any() ? rows.Average(x => x.HI) : 0d).ToString("F2", CultureInfo.InvariantCulture))
                .AppendLine("- 평균 PoF: " + (rows.Any() ? rows.Average(x => x.PoFRatio) * 100d : 0d).ToString("F2", CultureInfo.InvariantCulture) + "%")
                .AppendLine("- 총 Risk: $" + rows.Sum(x => x.Risk).ToString("N2", CultureInfo.InvariantCulture))
                .AppendLine("- 총 NPV: " + rows.Sum(x => x.NpvValue).ToString("N0", CultureInfo.InvariantCulture) + "원")
                .AppendLine("- System SAIDI: " + rows.Sum(x => x.SaidiContribution).ToString("F4", CultureInfo.InvariantCulture))
                .AppendLine("- System SAIFI: " + rows.Sum(x => x.SaifiContribution).ToString("F4", CultureInfo.InvariantCulture))
                .AppendLine()
                .AppendLine("## 자산·위험·의사결정 등록부")
                .AppendLine()
                .AppendLine("| 순위 | 설비코드 | 설비명 | HI | PoF(%) | Risk($) | BCR | RUL(년) | 의사결정 |")
                .AppendLine("|---:|---|---|---:|---:|---:|---:|---:|---|");
            foreach (DmDecisionInfo row in rows)
            {
                builder.Append("| ").Append(row.TopsisRank)
                    .Append(" | ").Append(row.Code)
                    .Append(" | ").Append((row.ProductName ?? "").Replace("|", "/"))
                    .Append(" | ").Append(row.HI.ToString("F2", CultureInfo.InvariantCulture))
                    .Append(" | ").Append((row.PoFRatio * 100d).ToString("F2", CultureInfo.InvariantCulture))
                    .Append(" | ").Append(row.Risk.ToString("N0", CultureInfo.InvariantCulture))
                    .Append(" | ").Append(row.Bcr.ToString("F3", CultureInfo.InvariantCulture))
                    .Append(" | ").Append(row.RULYears.GetValueOrDefault().ToString("F2", CultureInfo.InvariantCulture))
                    .Append(" | ").Append(row.Decision).AppendLine(" |");
            }
            builder.AppendLine()
                .AppendLine("## ISO 55001 정합 근거")
                .AppendLine()
                .AppendLine("- 6.2 자산관리 계획: AHP-TOPSIS 우선순위 및 5개년 Rolling-horizon 예산")
                .AppendLine("- 7.5 문서화된 정보: CSV·CIM JSON-LD·본 리포트로 계산 결과 기록")
                .AppendLine("- 8.1 운영계획 및 관리: 교체·정비·감시 액션과 예비품 조달계획")
                .AppendLine("- 9.1 모니터링·측정·분석: HI·PoF·4D CoF·Risk·SAIDI·SAIFI")
                .AppendLine("- 10.2 예방조치: 5개년 예측 및 민감도·불확실성 분석");

            return Utf8File(builder.ToString(), "text/markdown; charset=utf-8",
                "MVDC_AMS_ISO55001_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".md");
        }

        private static string Csv(object value)
        {
            string text;
            var formattable = value as IFormattable;
            if (formattable != null)
            {
                text = formattable.ToString(null, CultureInfo.InvariantCulture);
            }
            else
            {
                text = value == null ? string.Empty : Convert.ToString(value, CultureInfo.InvariantCulture);
            }

            // Spreadsheet formula injection prevention for exported text fields.
            if (text.StartsWith("=") || text.StartsWith("+")
                || text.StartsWith("-") || text.StartsWith("@"))
            {
                text = "'" + text;
            }

            return "\"" + text.Replace("\"", "\"\"") + "\"";
        }

        private static string CimClass(string assetType)
        {
            switch (assetType)
            {
                case "Transformer": return "PowerTransformer";
                case "VCB":
                case "Circuit_Breaker": return "Breaker";
                case "DC_Breaker": return "DCBreaker";
                case "Converter": return "ACDCConverter";
                case "MMC_Submodule": return "PowerElectronicsUnit";
                case "Cable": return "ACLineSegment";
                case "DC_Cable": return "DCLineSegment";
                case "Switchgear": return "Bay";
                case "Protection_Relay": return "ProtectionEquipment";
                case "SCADA": return "RemoteUnit";
                case "Energy_Storage": return "BatteryUnit";
                default: return string.Empty;
            }
        }

        private FileResult Utf8File(string text, string contentType, string fileName)
        {
            byte[] preamble = Encoding.UTF8.GetPreamble();
            byte[] body = Encoding.UTF8.GetBytes(text ?? string.Empty);
            return File(preamble.Concat(body).ToArray(), contentType, fileName);
        }
    }
}
