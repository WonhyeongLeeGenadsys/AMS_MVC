using AMS_MVC.Models;
using AMS_MVC.Repositories;
using LaModule;  // LaAlgorithm
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AMS_MVC.Controllers
{
    public class EquipmentAlgorithmController : Controller
    {
        private EquipmentWeibullRepository _weibullRepo = new EquipmentWeibullRepository();

        // 기존 5대
        private VCBBasicInfoRepository _vcbRepo = new VCBBasicInfoRepository();
        private DCCBBasicInfoRepository _dccbRepo = new DCCBBasicInfoRepository();
        private DCCABLEBasicInfoRepository _dccableRepo = new DCCABLEBasicInfoRepository();
        private ITRBasicInfoRepository _itrRepo = new ITRBasicInfoRepository();
        private SUBMODULEBasicInfoRepository _submoduleRepo = new SUBMODULEBasicInfoRepository();

        // 추가 15대
        private SABasicInfoRepository _saRepo = new SABasicInfoRepository();
        private DCCTBasicInfoRepository _dcctRepo = new DCCTBasicInfoRepository();
        private ARMREACTORBasicInfoRepository _armReactorRepo = new ARMREACTORBasicInfoRepository();
        private WALLBUSHINGBasicInfoRepository _wallBushingRepo = new WALLBUSHINGBasicInfoRepository();
        private PTBasicInfoRepository _ptRepo = new PTBasicInfoRepository();
        private CTBasicInfoRepository _ctRepo = new CTBasicInfoRepository();
        private LABasicInfoRepository _laRepo = new LABasicInfoRepository();
        private DSBasicInfoRepository _dsRepo = new DSBasicInfoRepository();
        private TANKBasicInfoRepository _tankRepo = new TANKBasicInfoRepository();
        private HEATEXCHANGERBasicInfoRepository _heatExchangerRepo = new HEATEXCHANGERBasicInfoRepository();
        private BYPASSVALVEBasicInfoRepository _bypassValveRepo = new BYPASSVALVEBasicInfoRepository();
        private PUMPBasicInfoRepository _pumpRepo = new PUMPBasicInfoRepository();
        private ZIGZAGTRBasicInfoRepository _zigZagTRRepo = new ZIGZAGTRBasicInfoRepository();
        private ESBasicInfoRepository _esRepo = new ESBasicInfoRepository();
        private NGRBasicInfoRepository _ngrRepo = new NGRBasicInfoRepository();

        /// <summary>
        /// 장비 유형(equipmentType)에 따른 B3 히스토그램 데이터를 반환합니다.
        /// </summary>
        [HttpGet]
        public ActionResult GetB3HistogramEquipment(string equipmentType = "VCB")
        {
            // 1) EquipmentWeibull 테이블에서 해당 장비 유형 데이터 가져오기
            var eqList = _weibullRepo.GetAll();
            var filteredWeibullList = eqList
                .Where(eq => eq.EquipmentName.ToUpper().Contains(equipmentType.ToUpper()))
                .ToList();

            if (filteredWeibullList.Count == 0)
            {
                return Json(new { error = "해당 장비의 Weibull 데이터가 없습니다." }, JsonRequestBehavior.AllowGet);
            }

            // 2) 첫 번째 항목의 shape/scale 혹은 FailureRate를 사용하여 B3 수명 계산
            var first = filteredWeibullList.FirstOrDefault(eq => eq.ShapeParam.HasValue && eq.ScaleParam.HasValue)
                        ?? filteredWeibullList.FirstOrDefault(eq => eq.FailureRate.HasValue);
            if (first == null)
            {
                return Json(new { error = "Weibull 또는 고장률 데이터가 없습니다." }, JsonRequestBehavior.AllowGet);
            }

            double b3 = 0;
            var algo = new LaAlgorithm();
            if (first.ShapeParam.HasValue && first.ScaleParam.HasValue)
            {
                algo.SetWeibull(first.ShapeParam.Value, first.ScaleParam.Value, 10);
                b3 = algo.B3Life;
            }
            else if (first.FailureRate.HasValue)
            {
                algo.SetFailureRate(first.FailureRate.Value);
                b3 = algo.B3Life;
            }

            List<dynamic> basicList;
            switch (equipmentType.Trim().ToUpper())
            {
                case "VCB":
                    {
                        _vcbRepo.GetAllVCBBasicInfoRepo(out List<VCBBasicInfo> list);
                        basicList = list.Cast<dynamic>().ToList();
                    }
                    break;
                case "DCCB":
                    {
                        _dccbRepo.GetAllDCCBBasicInfoRepo(out List<DCCBBasicInfo> list);
                        basicList = list.Cast<dynamic>().ToList();
                    }
                    break;
                case "DCCABLE":
                    {
                        _dccableRepo.GetAllDCCABLEBasicInfoRepo(out List<DCCABLEBasicInfo> list);
                        basicList = list.Cast<dynamic>().ToList();
                    }
                    break;
                case "ITR":
                    {
                        _itrRepo.GetAllITRBasicInfoRepo(out List<ITRBasicInfo> list);
                        basicList = list.Cast<dynamic>().ToList();
                    }
                    break;
                case "SUBMODULE":
                    {
                        _submoduleRepo.GetAllSUBMODULEBasicInfoRepo(out List<SUBMODULEBasicInfo> list);
                        basicList = list.Cast<dynamic>().ToList();
                    }
                    break;

                // --- 추가 15대 ---
                case "SA":
                    {
                        _saRepo.GetAllSABasicInfoRepo(out List<SABasicInfo> list);
                        basicList = list.Cast<dynamic>().ToList();
                    }
                    break;
                case "DCCT":
                    {
                        _dcctRepo.GetAllDCCTBasicInfoRepo(out List<DCCTBasicInfo> list);
                        basicList = list.Cast<dynamic>().ToList();
                    }
                    break;
                case "ARMREACTOR":
                    {
                        _armReactorRepo.GetAllARMREACTORBasicInfoRepo(out List<ARMREACTORBasicInfo> list);
                        basicList = list.Cast<dynamic>().ToList();
                    }
                    break;
                case "WALLBUSHING":
                    {
                        _wallBushingRepo.GetAllWALLBUSHINGBasicInfoRepo(out List<WALLBUSHINGBasicInfo> list);
                        basicList = list.Cast<dynamic>().ToList();
                    }
                    break;
                case "PT":
                    {
                        _ptRepo.GetAllPTBasicInfoRepo(out List<PTBasicInfo> list);
                        basicList = list.Cast<dynamic>().ToList();
                    }
                    break;
                case "CT":
                    {
                        _ctRepo.GetAllCTBasicInfoRepo(out List<CTBasicInfo> list);
                        basicList = list.Cast<dynamic>().ToList();
                    }
                    break;
                case "LA":
                    {
                        _laRepo.GetAllLABasicInfoRepo(out List<LABasicInfo> list);
                        basicList = list.Cast<dynamic>().ToList();
                    }
                    break;
                case "DS":
                    {
                        _dsRepo.GetAllDSBasicInfoRepo(out List<DSBasicInfo> list);
                        basicList = list.Cast<dynamic>().ToList();
                    }
                    break;
                case "TANK":
                    {
                        _tankRepo.GetAllTANKBasicInfoRepo(out List<TANKBasicInfo> list);
                        basicList = list.Cast<dynamic>().ToList();
                    }
                    break;
                case "HEATEXCHANGER":
                    {
                        _heatExchangerRepo.GetAllHEATEXCHANGERBasicInfoRepo(out List<HEATEXCHANGERBasicInfo> list);
                        basicList = list.Cast<dynamic>().ToList();
                    }
                    break;
                case "BYPASSVALVE":
                    {
                        _bypassValveRepo.GetAllBYPASSVALVEBasicInfoRepo(out List<BYPASSVALVEBasicInfo> list);
                        basicList = list.Cast<dynamic>().ToList();
                    }
                    break;
                case "PUMP":
                    {
                        _pumpRepo.GetAllPUMPBasicInfoRepo(out List<PUMPBasicInfo> list);
                        basicList = list.Cast<dynamic>().ToList();
                    }
                    break;
                case "ZIGZAGTR":
                    {
                        _zigZagTRRepo.GetAllZIGZAGTRBasicInfoRepo(out List<ZIGZAGTRBasicInfo> list);
                        basicList = list.Cast<dynamic>().ToList();
                    }
                    break;
                case "ES":
                    {
                        _esRepo.GetAllESBasicInfoRepo(out List<ESBasicInfo> list);
                        basicList = list.Cast<dynamic>().ToList();
                    }
                    break;
                case "NGR":
                    {
                        _ngrRepo.GetAllNGRBasicInfoRepo(out List<NGRBasicInfo> list);
                        basicList = list.Cast<dynamic>().ToList();
                    }
                    break;

                default:
                    return Json(new { error = "알 수 없는 장비 유형입니다." },
                                JsonRequestBehavior.AllowGet);
            }


            // 4) 각 장비의 가동일(Operating_Date)을 기준으로 사용기간(년) 계산
            var usageYears = new List<double>();
            foreach (var item in basicList)
            {
                double used = 0;
                if (item.Operating_Date != null)
                {
                    used = DateTime.Now.Year - ((DateTime)item.Operating_Date).Year;
                }
                usageYears.Add(used);
            }

            // 5) 히스토그램에 사용할 bin 생성 (x축 시간 범위는 LaAlgorithm의 TimeValues 최대값 사용)
            double timeMax = algo.TimeValues.Max();
            int binSize = 10;
            var bins = new List<dynamic>();
            for (int start = 0; start < (int)timeMax; start += binSize)
            {
                bins.Add(new { binStart = start, count = 0 });
            }

            // 6) 사용기간 데이터를 각 bin에 배분
            foreach (double usedYear in usageYears)
            {
                int index = (int)(usedYear / binSize);
                if (index >= bins.Count)
                    index = bins.Count - 1;
                var oldItem = bins[index];
                bins[index] = new { binStart = oldItem.binStart, count = (int)oldItem.count + 1 };
            }

            // 7) 결과 데이터 구성 후 반환
            var result = new
            {
                B3 = b3,
                BinSize = binSize,
                TimeMax = timeMax,
                Histogram = bins
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAlgorithmData(string equipmentType = "VCB")
        {
            // EquipmentWeibull 테이블에서 해당 장비 유형 데이터 가져오기
            var eqList = _weibullRepo.GetAll()
                                     .Where(eq => eq.EquipmentName.ToUpper().Contains(equipmentType.ToUpper()))
                                     .ToList();

            if (eqList.Count == 0)
            {
                return Json(new { error = "해당 장비의 Weibull 데이터가 없습니다." }, JsonRequestBehavior.AllowGet);
            }

            var resultList = new List<object>();

            foreach (var eq in eqList)
            {
                // 형상모수와 척도모수가 둘 다 없으면, 고장률 데이터가 있는지 확인하고 없다면 이 항목은 건너뜁니다.
                if (!eq.ShapeParam.HasValue || !eq.ScaleParam.HasValue)
                {
                    if (!eq.FailureRate.HasValue)
                    {
                        // 형상모수/척도모수도 없고 고장률 데이터도 없으므로 해당 장비는 처리하지 않음
                        continue;
                    }
                }

                var algo = new LaAlgorithm();

                // 형상모수와 척도모수가 있다면 이를 사용하여 Weibull 계산 수행
                if (eq.ShapeParam.HasValue && eq.ScaleParam.HasValue)
                {
                    algo.SetWeibull(eq.ShapeParam.Value, eq.ScaleParam.Value, 10);
                }
                // 형상모수/척도모수가 없고 고장률 데이터가 있는 경우라면 고장률을 이용하여 계산 수행
                else if (eq.FailureRate.HasValue)
                {
                    algo.SetFailureRate(eq.FailureRate.Value);
                }

                int length = algo.TimeValues.Length;
                var reliabilitySeries = new List<object>();
                var hazardSeries = new List<object>();
                var pdfSeries = new List<object>();

                for (int i = 0; i < length; i++)
                {
                    reliabilitySeries.Add(new
                    {
                        time = algo.TimeValues[i],
                        value = algo.Reliability[i]
                    });
                    hazardSeries.Add(new
                    {
                        time = algo.TimeValues[i],
                        value = algo.HazardNormalized[i]
                    });
                    pdfSeries.Add(new
                    {
                        time = algo.TimeValues[i],
                        value = algo.PdfNormalized[i]
                    });
                }

                resultList.Add(new
                {
                    EquipmentName = eq.EquipmentName,
                    B3Life = algo.B3Life,
                    ReliabilitySeries = reliabilitySeries,
                    HazardSeries = hazardSeries,
                    PdfNormalized = pdfSeries
                });
            }

            return Json(resultList, JsonRequestBehavior.AllowGet);
        }
    }
}
