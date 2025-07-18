using System;
using System.Linq;
using System.Threading.Tasks;
using InfluxDB.Client;
using NodaTime;
using NodaTime.Text;

namespace AMS_DATA
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var url = "http://192.168.0.24:8086";
            var token = "cOPC3HVD8zxxSWUs2go2zWaVx7NAVEjVoX3cCAKzLc_QZJeUFoCJxvYjS8dKynP5s37jsYsfjA0baQLFKpE64Q==";
            var org = "mvdc";
            var bucket = "AMS";

            var client = InfluxDBClientFactory.Create(url, token);

            var flux = $@"
            from(bucket: ""{bucket}"")
                |> range(start: -1000h)
                |> filter(fn: (r) => r._measurement =~ /YHLU.*/ )
            ";

            var tables = await client.GetQueryApi().QueryAsync(flux, org);

            foreach (var rec in tables.SelectMany(t => t.Records))
            {
                var measurement = rec.GetMeasurement();            // e.g. "YHLU01/SPDC1.ST.MoDevFlt."
                var parts = measurement.Split(new[] { '/' }, 2);
                var device = parts[0];                       // "YHLU01"
                var metricRaw = parts.Length > 1
                                  ? parts[1].TrimEnd('.')         // "SPDC1.ST.MoDevFlt"
                                  : "";

                var metricParts = metricRaw.Split(
                    new[] { '.' },
                    StringSplitOptions.RemoveEmptyEntries
                );

                var group = metricParts.ElementAtOrDefault(0) ?? "";
                var category = metricParts.ElementAtOrDefault(1) ?? "";
                var fieldName = metricParts.ElementAtOrDefault(2) ?? "";

                var timeText = rec.GetTime() is Instant inst
                    ? InstantPattern.ExtendedIso.Format(inst)
                    : "-";
                var value = rec.GetValue();

                Console.WriteLine(
                    $"[{device}] 그룹={group}, 카테고리={category}, 필드={fieldName} -> {timeText} = {value}"
                );
            }

            Console.WriteLine();
            Console.WriteLine("아무 키나 누르면 종료...");
            Console.ReadKey();

        }
    }
}

