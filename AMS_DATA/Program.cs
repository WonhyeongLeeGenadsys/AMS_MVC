using System;
using System.Linq;
using System.Threading.Tasks;
using InfluxDB.Client;
using NodaTime;                // Instant 타입 사용하기 위해 호출함
// using NodaTime.Text;       // (필요할 경우 InstantPattern 사용)

namespace AMS_DATA
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var url = "http://192.168.0.24:8086";
            var token = "cOPC3HVD8zxxSWUs2go2zWaVx7NAVEjVoX3cCAKzLc_QZJeUFoCJxvYjS8dKynP5s37jsYsfjA0baQLFKpE64Q==";
            var org = "mvdc"; //조직 이름
            var bucket = "AMS"; //버킷 이름(table)

            var client = InfluxDBClientFactory.Create(url, token); // url이랑 토큰으로  인스턴스화 

            // 모든 데이터 조회 + pivot(필드별 컬럼 분리)
            var flux = $@"
            from(bucket: ""{bucket}"")
                |> range(start: -24h)
                |> pivot(
                    rowKey:    [""_time""], 
                    columnKey: [""_field""],
                    valueColumn: ""_value""
                    )
            ";
            var tables = await client.GetQueryApi().QueryAsync(flux, org); //GetQueryApi: Flux 쿼리 API 객체, QueryAsync: 비동기로 flux 실행 -> tablses에 여러 테이블 단위로 결과 저장

            // 1) 모든 레코드 출력
            long printed = 0;
            foreach (var rec in tables.SelectMany(t => t.Records))
            {
                // Instant? 패턴 매칭 → ISO 포맷
                string timeText = rec.GetTime() is Instant inst
                    ? inst.ToDateTimeUtc().ToString("o")
                    : "-";

                // 딕셔너리에 들어있는 모든 컬럼(Key=Value) 출력
                var values = rec.Values
                                .Select(kv => $"{kv.Key}={kv.Value}")
                                .ToArray();
                Console.WriteLine($"{timeText} | {string.Join(", ", values)}");
                printed++;
            }

            // 마지막에 총 레코드 수 한 번만 출력
            Console.WriteLine();
            Console.WriteLine($"=== 총 레코드 수: {printed} ===");

            // 바로 종료되지 않도록 키 입력 대기
            Console.WriteLine("아무 키나 누르면 종료합니다...");
            Console.ReadKey();
        }
    }
}
