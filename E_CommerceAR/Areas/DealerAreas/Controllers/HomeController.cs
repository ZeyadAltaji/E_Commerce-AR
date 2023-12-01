using E_CommerceAR.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceAR.Areas.DealerAreas.Controllers
{
    [Area("DealerAreas")]

    public class HomeController : BaseController
    {
        [Route("DealerAreas/Home/Index")]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Container()
        {
            return PartialView("_Container");
        }
        public IActionResult Menu()
        {
            return PartialView("_MenuPartial");
        }
        public IActionResult NavBar()
        {
            return PartialView("_NavBarPartial");
        }
    }
}
