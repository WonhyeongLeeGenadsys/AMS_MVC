using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using InfluxDB.Client;
using NodaTime.Text;
using AMS_MVC.Models;              
using AMS_MVC.Repositories;        
using AMS_MVC.Services;            

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
        [Obsolete]
        static async Task Main(string[] args)
        {            
            var basicRepo = new VCBBasicInfoRepository();
            string latestCode = basicRepo.GetLatestVCBCode();          // EX) "VCB012"
            var basicInfo = basicRepo.GetVCBBasicInfoByCode(latestCode);

            Console.WriteLine($"▶ 처리할 VCB_CODE: {basicInfo.VCB_Code}, Serial: {basicInfo.Serial_No}");

            const string org = "mvdc";
            const string bucket = "AMS";
            const string url = "http://192.168.0.24:8086";
            const string token = "cOPC3HVD8zxxSWUs2go2zWaVx7NAVEjVoX3cCAKzLc_QZJeUFoCJxvYjS8dKynP5s37jsYsfjA0baQLFKpE64Q==";

            // db 연결
            var client = InfluxDBClientFactory.Create(url, token);

            // 1) 실시간 신호값 읽기 (YHLU 측정치 기준)
            var signals = await FetchSignals(client, org, bucket);

            // 2) VCBChk 모델 채우기
            var model = new VCBChk
            {
                VCB_Code = basicInfo.VCB_Code,
            };
            FillModelFromSignals(model, signals);

            // 3) 점검 알고리즘 실행
            var calc = new VCBChkScoreCalculator();
            var (hi, pof) = calc.CalculateHiPof(model, alpha: 0.99m);
            model.FoldingFunction = (int)Math.Round(hi);

            // 4) 결과 화면 출력 및 DB 저장
            Console.WriteLine($"HI={hi:F2}, POF={pof:F4}, FoldingFunction={model.FoldingFunction}");
            var chkRepo = new VCBChkRepository();
            var res = chkRepo.CreateVCBChkRepo(model);
            Console.WriteLine($"DB 저장: Success={res.IsSuccess}, Msg={res.Message}");

            Console.WriteLine("아무 키나 누르면 종료");
            Console.ReadKey();
        }
        
        static async Task<Dictionary<string, float>> FetchSignals(
            InfluxDBClient client,
            string org,
            string bucket
        )

        {
            var flux = $@"
            from(bucket: ""{bucket}"")
                |> range(start: -1000h)
                
                |> filter(fn: (r) => r._measurement =~ /AcItrInfo.*/ and r._field == ""value""
            ";

            //|> filter(fn: (r) => r._measurement = ~ / YHLU.*/ and r._field == ""value""

            var tables = await client.GetQueryApi().QueryAsync(flux, org);
            var dict = new Dictionary<string, float>();
            foreach (var rec in tables.SelectMany(t => t.Records))
            {
                var meas = rec.GetMeasurement();  
                if (float.TryParse(rec.GetValue().ToString(), out var v))
                    dict[meas] = v;
            }
            return dict;
        }
        
        static void FillModelFromSignals(VCBChk model, Dictionary<string, float> signals)
        {
            var props = typeof(VCBChk)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(float))
                .ToDictionary(p => p.Name, p => p);

            foreach (var kv in signals)
            {
                if (!InfluxSignalMapper.Map.TryGetValue(kv.Key, out var propName))
                    continue;
                if (props.TryGetValue(propName, out var prop))
                    prop.SetValue(model, kv.Value);
            }
        }
    }
}
