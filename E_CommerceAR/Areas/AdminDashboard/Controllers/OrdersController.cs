using E_CommerceAR.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceAR.Areas.AdminDashboard.Controllers
{
    public class OrdersController : BaseController
    {
        public OrdersController()
        {
            
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
