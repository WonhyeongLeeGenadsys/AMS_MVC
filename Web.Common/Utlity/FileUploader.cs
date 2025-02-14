using System;
using System.IO;
using System.Web;

namespace Web.Common.Utlity
{
    public class FileUploader
    {
        private readonly string _uploadFolder;
        private readonly string _serverPath;

        public FileUploader(string uploadFolder, string serverPath)
        {
            _uploadFolder = uploadFolder;
            _serverPath = serverPath;

            if (!Directory.Exists(_serverPath))
            {
                Directory.CreateDirectory(_serverPath);
            }
        }

        public string UploadFile(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                throw new Exception("파일이 전송되지 않았습니다.");
            }

            // 원래 파일명에서 확장자 추출
            string originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
            string extension = Path.GetExtension(file.FileName);
            string fileName = originalFileName;
            string filePath = Path.Combine(_serverPath, fileName + extension);
            int count = 1;

            // 동일 이름 파일이 있을 경우 새로운 이름 생성
            while (File.Exists(filePath))
            {
                fileName = $"{originalFileName}({count})";
                filePath = Path.Combine(_serverPath, fileName + extension);
                count++;
            }

            file.SaveAs(filePath);
            // URL 경로 형식으로 변환 (백슬래시를 슬래시로)
            return Path.Combine(_uploadFolder, fileName + extension).Replace("\\", "/");
        }
    }
}
