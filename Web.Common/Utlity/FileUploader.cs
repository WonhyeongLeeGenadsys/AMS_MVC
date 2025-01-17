using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public string UploadFile(HttpPostedFile file)
        {
            if (file == null || file.ContentLength == 0)
            {
                throw new Exception("파일이 전송되지 않았습니다.");
            }

            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
            string extension = Path.GetExtension(file.FileName);
            string filePath = Path.Combine(_serverPath, fileName + extension);
            int count = 1;

            while (File.Exists(filePath))
            {
                fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}({count})";
                filePath = Path.Combine(_serverPath, fileName + extension);
                count++;
            }

            file.SaveAs(filePath);
            return Path.Combine(_uploadFolder, fileName + extension).Replace("\\", "/");
        }
    }
}
