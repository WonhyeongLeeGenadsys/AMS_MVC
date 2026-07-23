using System.Web.Mvc;
using Web.Common;

namespace AMS_MVC
{
    public partial class SPAREBasicController : Controller
    {
        private readonly SPAREBasicInfoRepository spareBasicRepository;

        public SPAREBasicController()
        {
            ViewBag.MenuType = "SPARE";
            spareBasicRepository = new SPAREBasicInfoRepository();
        }
    }
}