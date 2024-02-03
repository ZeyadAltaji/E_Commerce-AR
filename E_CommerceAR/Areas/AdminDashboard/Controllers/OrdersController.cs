using E_CommerceAR.Controllers;
using E_CommerceAR.Domain.ModalsBase;
using E_CommerceAR.Domain.ModalsViews;
using Firebase.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using static E_CommerceAR.Areas.AdminDashboard.Controllers.ProductsController;
using static Google.Cloud.Firestore.V1.StructuredQuery.Types;

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
        [Route("Orders/Index")]

        public IActionResult Index()
        {
            return View();
        }
        [Route("Orders/ListOrders")]

        [HttpGet]
        public async Task<IActionResult> ListOrders()
        {
			try
			{
                List<OrdersViewModel> productDataList = await FetchOrdersFromDatabase();

                 

				return PartialView(productDataList);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error fetching orders: {ex.Message}");
				return StatusCode(500, "Internal Server Error");
			}
		}
        private async Task<List<OrdersViewModel>> FetchOrdersFromDatabase()
        {
            try
            {
                CollectionReference productsCollection = firestoreDb.Collection("orders");
                QuerySnapshot querySnapshot = await productsCollection.GetSnapshotAsync();

                List<OrdersViewModel> OrdersList = new List<OrdersViewModel>();

                foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
                {
                    try
                    {
                        Orders Order = documentSnapshot.ConvertTo<Orders>();
                        string documentPath = documentSnapshot.Reference.Path;
                        string documentId = documentPath.Split('/').Last();
                        OrdersList.Add(new OrdersViewModel { Orders = Order, DocumentId = documentId });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting document {documentSnapshot.Id}: {ex.Message}");
                    }
                }

                return OrdersList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching products: {ex.Message}");
                return new List<OrdersViewModel>();
            }
        }
        [Route("Orders/ViewOrder")]
        public IActionResult ViewOrder(string DocumentId)
        {
            CollectionReference ViewOrderCollection = firestoreDb.Collection("orders");

             DocumentSnapshot snapshot = ViewOrderCollection.Document(DocumentId).GetSnapshotAsync().Result;

             if (snapshot.Exists)
             {
                 Orders order = snapshot.ConvertTo<Orders>();

                 return PartialView(order);
             }
            else
            {
                 return NotFound();
            }
        }
    }
}
