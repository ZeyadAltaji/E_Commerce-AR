using E_CommerceAR.Controllers;
using E_CommerceAR.Domain.ModalsBase;
using E_CommerceAR.Domain.ModalsViews;
using Firebase.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceAR.Areas.DealerAreas.Controllers
{
    [Area("DealerAreas")]

    public class HomeController : BaseController
    {
        FirebaseAuthProvider auth;
        private readonly FirestoreDb firestoreDb;


        public HomeController()
        {
            auth = new FirebaseAuthProvider(
                                      new FirebaseConfig(ApiKey));

            firestoreDb = FirestoreDb.Create(PorjectId);
        }
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


        #region Area Counts
        private async Task<int> CountProducts()
        {
            try
            {
                Query productsCollection = firestoreDb.Collection("Products").WhereEqualTo("dealerId", DocumentId);
                QuerySnapshot productSnapshot = await productsCollection.GetSnapshotAsync();

                return productSnapshot.Count;
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"Error counting products: {ex.Message}");
                return 0;
            }
        }
        private async Task<int> CountOrders()
        {
            try
            {
                Query ordersCollection = firestoreDb.Collection("orders");
                QuerySnapshot orderSnapshot = await ordersCollection.GetSnapshotAsync();

                return orderSnapshot.Count;
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"Error counting orders: {ex.Message}");
                return 0;
            }
        }
        private async Task<decimal> CalculateTotalRevenue()
        {
            List<Orders> ordersList = await FetchOrdersFromDatabase();
            decimal totalRevenue = (decimal)ordersList.Sum(order => order.TotalPrice);
            return totalRevenue;
        }

        private Task<List<Orders>> FetchOrdersFromDatabase()
        {
            throw new NotImplementedException();
        }
        private async Task<decimal> CalculateTotalCost()
        {
            List<Product> productsList = await FetchProductsFromDatabase();
            decimal totalCost = (decimal)productsList.Sum(product => product.Price ?? 0);
            return totalCost;
        }

        private async Task<List<Product>> FetchProductsFromDatabase()
        {
            try
            {
                Query productsCollection = firestoreDb.Collection("Products").WhereEqualTo("dealerId", DocumentId);
                QuerySnapshot querySnapshot = await productsCollection.GetSnapshotAsync();

                List<Product> productList = new List<Product>();

                foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
                {
                    try
                    {
                        Product product = documentSnapshot.ConvertTo<Product>();
                        productList.Add(product);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting document {documentSnapshot.Id}: {ex.Message}");
                    }
                }

                return productList;
            }
            catch (Exception ex)
            {
                // Handle exception
                throw new NotImplementedException();
            }
        }
        private decimal CalculateProfitRatio(decimal totalRevenue, decimal totalCost)
        {
            if (totalRevenue == 0)
            {
                return 0;
            }

            decimal profit = totalRevenue - totalCost;
            decimal profitRatio = (profit / totalRevenue) * 100;

            return profitRatio;
        }

        public async Task<(int, List<OrdersViewModel>)> GetDeliveredOrders()
        {
            try
            {
                List<OrdersViewModel> ordersList = await FetchOrdersFromDocuments();
                List<OrdersViewModel> deliveredOrders = ordersList
                    .Where(order => order.Orders.OrderStatus == "Delivered")
                    .ToList();
                int deliveredOrdersCount = deliveredOrders.Count;
                return (deliveredOrdersCount, deliveredOrders);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting delivered orders: {ex.Message}");
                return (0, new List<OrdersViewModel>());
            }
        }

        private async Task<List<OrdersViewModel>> FetchOrdersFromDocuments()
        {
            try
            {
                CollectionReference productsCollection = firestoreDb.Collection("orders");
                QuerySnapshot querySnapshot = await productsCollection.GetSnapshotAsync();

                List<OrdersViewModel> ordersList = new List<OrdersViewModel>();

                foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
                {
                    try
                    {
                        Orders Order = documentSnapshot.ConvertTo<Orders>();
                        string documentPath = documentSnapshot.Reference.Path;
                        string documentId = documentPath.Split('/').Last();
                        ordersList.Add(new OrdersViewModel { Orders = Order, DocumentId = documentId });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting document {documentSnapshot.Id}: {ex.Message}");
                    }
                }

                return ordersList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching products: {ex.Message}");
                return new List<OrdersViewModel>();
            }
        }
        #endregion
    }
}
