
using System.Linq;
using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public class SettingController : Controller
    {
        private EquipmentWeibullRepository weibullRepo = new EquipmentWeibullRepository();
        private readonly CoFRepository cofRepo = new CoFRepository();
        private readonly CoFCalculator calculator = new CoFCalculator();

        // GET: Setting/SubstationInfo
        public ActionResult SubstationInfo()
        {
            ViewBag.MenuType = "Setting";

            var eqList = weibullRepo.GetAll();

            var vcb = eqList.FirstOrDefault(x => x.EquipmentName.ToUpper() == "VCB");
            var itr = eqList.FirstOrDefault(x => x.EquipmentName.ToUpper() == "ITR");
            var submodule = eqList.FirstOrDefault(x => x.EquipmentName.ToUpper() == "SUBMODULE");
            var dccb = eqList.FirstOrDefault(x => x.EquipmentName.ToUpper() == "DCCB");
            var dccable = eqList.FirstOrDefault(x => x.EquipmentName.ToUpper() == "DCCABLE");

            ViewBag.VCB = vcb;
            ViewBag.ITR = itr;
            ViewBag.SUBMODULE = submodule;
            ViewBag.DCCB = dccb;
            ViewBag.DCCABLE = dccable;

            return View("~/Views/Setting/SubstationInfo.cshtml");
        }

        public ActionResult MemberInfo()
        {
            ViewBag.MenuType = "Setting";
            return View("~/Views/Setting/MemberInfo.cshtml");
        }

        // GET: Setting/CofInfo
        public ActionResult CofInfo(string code = "VCB")
        {
            ViewBag.MenuType = "Setting";

            var equipmentTypes = new[] { "VCB", "ITR", "DCCB", "DCCABLE", "SUBMODULE" };
            ViewBag.EquipmentTypes = new SelectList(equipmentTypes, code);

            var model = cofRepo.GetLatest(code); // 없으면 내부에서 Code만 채운 새 모델 반환
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CofInfo(COFModel model)
        {
            ViewBag.MenuType = "Setting";
            var equipmentTypes = new[] { "VCB", "ITR", "DCCB", "DCCABLE", "SUBMODULE" };
            ViewBag.EquipmentTypes = new SelectList(equipmentTypes, model.Code);

            string[] computedKeys = {
            nameof(model.Customer_Power_Outage_Cost),
            nameof(model.System_Loss_Cost),
            nameof(model.Facility_Recovery_Cost),
            nameof(model.Loss_Of_Profit),
            nameof(model.Safety_Accident_Compensation_1),
            nameof(model.Safety_Accident_Compensation_2),
            nameof(model.Total_Cof)
            };
            foreach (var k in computedKeys) ModelState.Remove(k);

            if (!ModelState.IsValid)
            {
                // 검증 오류 있으면 그대로 입력값 다시 보여줌
                return View(model);
            }

            try
            {
                // 계산
                calculator.Calculate(model);

                // 저장 (오늘 날짜와 다르면 INSERT, 같으면 UPDATE)
                cofRepo.SaveOrUpdate(model);

                // 리스크매트릭스에는 고장확률을 적용하지 않은 기본 CoF를 반영한다.
                var riskRepo = new RiskmatrixRepository();
                riskRepo.UpdateCoFByPrefix(model.Code, model.Total_Cof);

                // 최신값 화면으로
                return RedirectToAction(nameof(CofInfo), new { code = model.Code });
            }
            catch (System.InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            catch (System.Exception ex)
            {
                LogHelper.WriteLog("Setting(CofInfo)", ex.ToString());
                ModelState.AddModelError("", "CoF 정보를 저장하는 중 오류가 발생했습니다. 입력값을 확인해 주세요.");
                return View(model);
            }
        }

        [HttpGet]
        public JsonResult GetCofData(string code)
        {
            // 오늘자 최신 모델 또는 새 빈 모델
            var model = cofRepo.GetLatest(code) ?? new COFModel { Code = code };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
