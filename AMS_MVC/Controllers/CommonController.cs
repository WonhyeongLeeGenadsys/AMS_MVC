using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

public class CommonController : Controller
{
    [HttpGet]
    public async Task<JsonResult> GetUsdKrwRate()
    {
        try
        {
            string apiKey = ConfigurationManager.AppSettings["KoreaExim.ApiKey"];
            using (var client = new HttpClient())
            {
                for (int i = 0; i < 7; i++)
                {
                    string date = DateTime.Now.AddDays(-i).ToString("yyyyMMdd");
                    string url =
                        "https://oapi.koreaexim.go.kr/site/program/financial/exchangeJSON" +
                        $"?authkey={apiKey}&searchdate={date}&data=AP01";

                    var json = await client.GetStringAsync(url);
                    var arr = JArray.Parse(json);

                    if (arr.Count == 0 || (int)arr[0]["result"] != 1)
                        continue;

                    var usd = arr.FirstOrDefault(x => (string)x["cur_unit"] == "USD");
                    if (usd == null)
                        continue;

                    decimal rate = decimal.Parse(
                        ((string)usd["deal_bas_r"]).Replace(",", "")
                    );

                    return Json(new { rate, baseDate = date }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { rate = 0 }, JsonRequestBehavior.AllowGet);
        }
        catch
        {
            Response.StatusCode = 500;
            return Json(new { rate = 0 }, JsonRequestBehavior.AllowGet);
        }
    }
}
