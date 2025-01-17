using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Common.Log
{
    public static class LogHelper
    {
        private static string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        private static string logFileName = $"{DateTime.Now:yyyy-MM-dd}_MVDC_Log.txt";
        private static string logFilePath = Path.Combine(logDirectory, logFileName);
        private const long MaxLogFileSize = 1 * 1024 * 1024; // 1MB

        public static void WriteLog(string location, string format, params object[] args)
        {
            string message = string.Format(format, args);
            string logEntry = $"|{DateTime.Now:yyyy-MM-dd HH:mm:ss}  |   {location}  |   {message}   |";

            try
            {
                // Logs 폴더가 없을 경우 생성
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                // 파일 크기 확인 및 필요 시 새 파일 생성
                ManageLogFileSize();

                // 로그 파일에 기록 (추가 모드로 작성)
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(logEntry);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("로그 작성 중 오류 발생: " + ex.Message);
            }
        }

        /// <summary>
        /// 파일 크기를 확인하고, 크기가 초과하면 새로운 파일 생성
        /// </summary>
        private static void ManageLogFileSize()
        {
            FileInfo fileInfo = new FileInfo(logFilePath);

            if (fileInfo.Exists && fileInfo.Length > MaxLogFileSize)
            {
                // 동일한 날짜의 파일이 여러 개 있는 경우 번호를 붙여서 새 파일명 지정
                int fileCount = 1;
                string newLogFileName;
                string newLogFilePath;

                do
                {
                    fileCount++;
                    newLogFileName = $"{DateTime.Now:yyyy-MM-dd}_MVDC_Log({fileCount}).txt";
                    newLogFilePath = Path.Combine(logDirectory, newLogFileName);
                } while (System.IO.File.Exists(newLogFilePath));

                logFilePath = newLogFilePath;
            }
        }
    }
}
