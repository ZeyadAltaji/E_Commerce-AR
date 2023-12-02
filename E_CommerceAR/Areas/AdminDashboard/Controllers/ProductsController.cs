using E_CommerceAR.Controllers;
using E_CommerceAR.Domain.ModalsViews;
using Firebase.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace E_CommerceAR.Areas.AdminDashboard.Controllers
{
    [Area("AdminDashboard")]

    public class ProductsController : BaseController
    {
        FirebaseAuthProvider auth;
        private readonly FirestoreDb firestoreDb;

        public ProductsController()
        {
            auth = new FirebaseAuthProvider(
                    new FirebaseConfig(ApiKey));
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
                "C:\\Users\\ziada\\Source\\repos\\E_CommerceAR\\E_CommerceAR\\Extensions\\" +
                     "finalprojectar-d85ea-firebase-adminsdk-9x4fl-3f47b05b2e.json");
            firestoreDb = FirestoreDb.Create(PorjectId);
        }
         [Route("Products/Index")]

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [Route("Products/ListProduct")]

        [HttpGet]
        public async Task<IActionResult> ListProduct()
        {
            CollectionReference productsCollection = firestoreDb.Collection("Products");
            QuerySnapshot querySnapshot = await productsCollection.GetSnapshotAsync();

            List<Products> products = new List<Products>();

            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                try
                {
                    Products product = documentSnapshot.ConvertTo<Products>();
                    products.Add(product);
                }
                catch (Exception ex)
                {
                     Console.WriteLine($"Error converting document {documentSnapshot.Id}: {ex.Message}");
                 }
            }

            return PartialView(products);
        }

        public IActionResult GetProductById(int id)
        {
            return PartialView();
        }
        public IActionResult AddNewProduct()
        {
            return PartialView();
        }
        public IActionResult EditProduct()
        {
            return PartialView();
        }
        public JsonResult DeleteProduct() 
        {
            return null ;
        }
    }
}
