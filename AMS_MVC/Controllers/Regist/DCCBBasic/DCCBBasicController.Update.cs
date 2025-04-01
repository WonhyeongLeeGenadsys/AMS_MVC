using AMS_MVC.Models;
using AMS_MVC.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common.Log;

namespace AMS_MVC.Controllers
{
    public partial class DCCBBasicController : Controller
    {
        // GET: 수정 폼 로드 (tblIdx에 해당하는 기존 데이터 조회)
        [HttpGet]
        public ActionResult UpdateDCCBBasic(string tblIdx)
        {
            var model = dccbBasicRepository.GetDCCBBasicInfoByTblIdxRepo(tblIdx);
            if (model == null)
            {
                return HttpNotFound("DCCB 기본정보를 찾을 수 없습니다.");
            }

            // 제작사 Dropdown에 사용할 데이터를 ViewBag에 담기
            var companies = new List<Company>();
            if (companyRepository.GetAllCompanies(out companies).IsSuccess && companies != null)
            {
                ViewBag.Companies = companies;
            }
            else
            {
                ViewBag.ErrorMessage = "제작사 정보를 불러올 수 없습니다.";
            }

            return View("~/Views/Regist/DCCB/DCCBBasicUpdate.cshtml", model);
        }

        // POST: 수정 요청 처리
        [HttpPost]
        public ActionResult UpdateDCCBBasicInfo(DCCBBasicInfo model, HttpPostedFileBase photo)
        {
            // 파일 업로드 처리 (새로운 사진이 업로드되었을 경우)
            if (photo != null && photo.ContentLength > 0)
            {
                var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var ext = Path.GetExtension(photo.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                {
                    return Json(new { success = false, message = "지원되지 않는 파일 형식입니다." });
                }

                var uploadsFolder = Server.MapPath("~/Uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                } 

                string originalFileName = Path.GetFileName(photo.FileName);
                string fileName = originalFileName;
                string filePath = Path.Combine(uploadsFolder, fileName);
                int counter = 1;
                while (System.IO.File.Exists(filePath))
                {
                    string fileNameWithoutExt = Path.GetFileNameWithoutExtension(originalFileName);
                    fileName = $"{fileNameWithoutExt}({counter}){ext}";
                    filePath = Path.Combine(uploadsFolder, fileName);
                    counter++;
                }
                photo.SaveAs(filePath);
                model.Photo = $"/Uploads/{fileName}";
            }
            // 만약 사진을 새로 업로드하지 않았다면 기존 사진 경로를 유지
            // 필요에 따라 기존 정보를 다시 조회하여 model.Photo에 할당
            // repository를 이용한 업데이트 처리
            var result = dccbBasicRepository.UpdateDCCBBasicInfoRepo(model);
            return Json(new { success = result.IsSuccess, message = result.Message });
        }
    }
}
