using Microsoft.AspNetCore.Mvc;

namespace E_CommerceAR.Areas.AdminDashboard.Controllers
{
    [Area("AdminDashboard")]

    public class HomeController : Controller
    {
         [Route("AdminDashboard/Home/Index")]
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
