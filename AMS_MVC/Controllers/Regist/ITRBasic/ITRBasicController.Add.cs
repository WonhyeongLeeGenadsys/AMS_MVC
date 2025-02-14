using AMS_MVC.Models;
using AMS_MVC.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers
{
    public partial class ITRBasicController : Controller
    {
        [HttpGet]
        public ActionResult ITRBasicAdd()
        {
            var companies = new List<Company>();
            if (companyRepository.GetAllCompanies(out companies).IsSuccess && companies != null)
            {
                ViewBag.Companies = companies;
            }
            else
            {
                ViewBag.ErrorMessage = "제작사 정보를 불러올 수 없습니다.";
            }
            return View("~/Views/Regist/ITR/ITRBasicAdd.cshtml");
        }

        [HttpPost]
        public ActionResult ITRBasicAdd(ITRBasicInfo model, HttpPostedFileBase photo)
        {
            // 이미지 업로드 처리 
            if (photo != null && photo.ContentLength > 0)
            {
                // 업로드할 파일 확장자 제한함
                var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var ext = Path.GetExtension(photo.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                {
                    return Json(new { success = false, message = "지원되지 않는 파일 형식입니다." });
                }

                // Uploads 폴더 경로 구하기
                var uploadsFolder = Server.MapPath("~/Uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // 원본 파일 이름 사용
                string originalFileName = Path.GetFileName(photo.FileName);
                string fileName = originalFileName;
                string filePath = Path.Combine(uploadsFolder, fileName);

                // 같은 이름의 파일이 있으면 (1), (2) 등을 붙여서 고유한 이름 생성
                int counter = 1;
                while (System.IO.File.Exists(filePath))
                {
                    // Ex), "image.jpg" -> "image(1).jpg"
                    string fileNameWithoutExt = Path.GetFileNameWithoutExtension(originalFileName);
                    fileName = $"{fileNameWithoutExt}({counter}){ext}";
                    filePath = Path.Combine(uploadsFolder, fileName);
                    counter++;
                }

                // 파일 저장
                photo.SaveAs(filePath);

                // 웹에서 접근 가능한 URL 경로 저장
                model.Photo = $"/Uploads/{fileName}";
            }
            else
            {
                // 파일이 선택되지 않았을 경우 기본 이미지나 null 처리
                model.Photo = null;
            }

            // 기타 데이터 처리 (ITR_CODE 생성 등)
            ITRBasicInfoRepository repo = new ITRBasicInfoRepository();
            string latestCode = repo.GetLatestITRCode();
            model.ITR_Code = GenerateNextITRCode(latestCode, "I");
            model.Writer = "TestUser";
            var result = repo.CreateITRBasicInfoRepo(model);

            return Json(new { success = result.IsSuccess, message = result.Message });
        }

        private static string GenerateNextITRCode(string latestCode, string prefix)
        {
            int codeNumber = 0;

            if (!string.IsNullOrEmpty(latestCode) && latestCode.StartsWith(prefix))
            {
                string numericPart = latestCode.Substring(prefix.Length);
                int.TryParse(numericPart, out codeNumber);
            }

            codeNumber += 1;
            return $"{prefix}{codeNumber:D3}";
        }
    }
}