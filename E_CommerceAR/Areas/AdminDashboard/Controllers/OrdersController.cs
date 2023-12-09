using E_CommerceAR.Controllers;
using Firebase.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using static E_CommerceAR.Areas.AdminDashboard.Controllers.ProductsController;

namespace E_CommerceAR.Areas.AdminDashboard.Controllers
{
    [Area("AdminDashboard")]

    public class OrdersController : BaseController
    {
        FirebaseAuthProvider auth;
        private readonly FirestoreDb firestoreDb;

        public OrdersController()
        {
            auth = new FirebaseAuthProvider(
                    new FirebaseConfig(ApiKey));
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
                "C:\\Users\\ziada\\Source\\repos\\E_CommerceAR\\E_CommerceAR\\Extensions\\" +
                     "finalprojectar-d85ea-firebase-adminsdk-9x4fl-3f47b05b2e.json");
            firestoreDb = FirestoreDb.Create(PorjectId);
        }
        
        public IActionResult Index()
        {
            return View();
        }
        [Route("Orders/ListOrders")]

        [HttpGet]
        public async Task<IActionResult> ListProduct()
        {
            //List<ProductViewModel> productDataList = await FetchProductsFromDatabase();
            //return PartialView(productDataList);
            return null;
        }
    }
}
