using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public class CommonController : Controller
    {
        private static readonly HttpClient ExchangeRateClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(5)
        };

        private static readonly SemaphoreSlim ExchangeRateLock = new SemaphoreSlim(1, 1);
        private static decimal _cachedUsdKrwRate;
        private static DateTime _cachedAtUtc = DateTime.MinValue;

        [HttpGet]
        public async Task<JsonResult> GetUsdKrwRate()
        {
            var cacheHours = ReadPositiveDecimalSetting("UsdKrwCacheHours", 12m);
            var cacheDuration = TimeSpan.FromHours((double)cacheHours);

            if (_cachedUsdKrwRate > 0m && DateTime.UtcNow - _cachedAtUtc < cacheDuration)
            {
                return ExchangeRateJson(_cachedUsdKrwRate, "cache", false);
            }

            await ExchangeRateLock.WaitAsync();
            try
            {
                if (_cachedUsdKrwRate > 0m && DateTime.UtcNow - _cachedAtUtc < cacheDuration)
                {
                    return ExchangeRateJson(_cachedUsdKrwRate, "cache", false);
                }

                try
                {
                    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
                    var apiUrl = ConfigurationManager.AppSettings["UsdKrwApiUrl"];
                    if (string.IsNullOrWhiteSpace(apiUrl))
                    {
                        apiUrl = "https://api.frankfurter.dev/v2/rate/USD/KRW";
                    }

                    var response = await ExchangeRateClient.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();
                    var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                    var rate = json.Value<decimal?>("rate") ?? 0m;
                    if (rate <= 0m)
                    {
                        throw new InvalidOperationException("USD/KRW 환율 응답값이 올바르지 않습니다.");
                    }

                    _cachedUsdKrwRate = rate;
                    _cachedAtUtc = DateTime.UtcNow;
                    return ExchangeRateJson(rate, "api", false);
                }
                catch (Exception ex)
                {
                    if (_cachedUsdKrwRate > 0m)
                    {
                        return ExchangeRateJson(_cachedUsdKrwRate, "stale-cache", true);
                    }

                    var fallbackRate = ReadPositiveDecimalSetting("UsdKrwFallbackRate", 1400m);
                    return Json(new
                    {
                        success = true,
                        rate = fallbackRate,
                        source = "fallback",
                        isFallback = true,
                        message = ex.Message
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            finally
            {
                ExchangeRateLock.Release();
            }
        }

        private JsonResult ExchangeRateJson(decimal rate, string source, bool isFallback)
        {
            return Json(new
            {
                success = true,
                rate,
                source,
                isFallback,
                cachedAt = _cachedAtUtc == DateTime.MinValue ? (DateTime?)null : _cachedAtUtc
            }, JsonRequestBehavior.AllowGet);
        }

        private static decimal ReadPositiveDecimalSetting(string key, decimal fallback)
        {
            decimal value;
            return decimal.TryParse(
                       ConfigurationManager.AppSettings[key],
                       NumberStyles.Number,
                       CultureInfo.InvariantCulture,
                       out value) && value > 0m
                ? value
                : fallback;
        }
    }
}
