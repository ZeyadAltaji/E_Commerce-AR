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
         {
                auth = new FirebaseAuthProvider(
                            new FirebaseConfig(ApiKey));
                System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
                    "C:\\Users\\ziada\\Source\\repos\\E_CommerceAR\\E_CommerceAR\\Extensions\\" +
                         "finalprojectar-d85ea-firebase-adminsdk-9x4fl-3f47b05b2e.json");
                firestoreDb = FirestoreDb.Create(PorjectId);
            }
        }
        [Route("DealerAreas/Home/Index")]
        public async Task<IActionResult> Index ()
        {
            int orderCount = await CountOrders();
            int productCount = await CountProducts();
            decimal totalRevenue = await CalculateTotalRevenue();
            decimal totalCost = await CalculateTotalCost();
            decimal profitRatio = CalculateProfitRatio(totalRevenue , totalCost);
            (int deliveredOrdersCount, List<OrdersViewModel> deliveredOrders) = await GetDeliveredOrders();
            ViewBag.OrderCount = orderCount;
            ViewBag.ProductCount = productCount;
            ViewBag.ProfitRatio = profitRatio;
            ViewBag.DeliveredOrdersCount = deliveredOrdersCount;
            ViewBag.DeliveredOrders = deliveredOrders;
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
                var DocumentId = HttpContext.Session.GetString("UserId");

                Query productsCollection = firestoreDb.Collection("Products").WhereEqualTo("dealerId", DocumentId);
                QuerySnapshot productSnapshot = await productsCollection.GetSnapshotAsync();

                return productSnapshot.Count;
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Error counting products: {ex.Message}");
                return 0;
            }
        }
        private async Task<int> CountOrders ()
        {
            try
            {
                var dealerId = HttpContext.Session.GetString("UserId");

                Query ordersQuery = firestoreDb.Collection("orders");
                QuerySnapshot querySnapshot = await ordersQuery.GetSnapshotAsync();

                List<Orders> ordersList = new List<Orders>();

                foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
                {
                    try
                    {
                        Orders order = documentSnapshot.ConvertTo<Orders>();

                        List<OrderItem> filteredOrderItems = order.Products
                            .Where(item => item.Product != null && item.Product.dealerId == dealerId)
                            .ToList();

                        if (filteredOrderItems.Count == 0)
                            continue;

                        order.Products = filteredOrderItems;
                        ordersList.Add(order);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting document {documentSnapshot.Id}: {ex.Message}");
                    }
                }

                return ordersList.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error counting orders: {ex.Message}");
                return 0;
            }
        }

        private async Task<decimal> CalculateTotalRevenue ()
        {
            List<Orders> ordersList = await FetchOrdersFromDatabase();
            decimal totalRevenue = 0;

            foreach (var order in ordersList)
            {
                foreach (var orderItem in order.Products)
                {
                     totalRevenue += (decimal)(orderItem.Product?.Price ?? 0) * orderItem.Quantity;
                }
            }

            return totalRevenue;
        }

        private async Task<List<Orders>> FetchOrdersFromDatabase ()
        {
            try
            {
                var dealerId = HttpContext.Session.GetString("UserId");

                Query ordersQuery = firestoreDb.Collection("orders");
                QuerySnapshot querySnapshot = await ordersQuery.GetSnapshotAsync();

                List<Orders> ordersList = new List<Orders>();

                foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
                {
                    try
                    {
                        Orders order = documentSnapshot.ConvertTo<Orders>();

                         List<OrderItem> filteredOrderItems = order.Products
                            .Where(item => item.Product != null && item.Product.dealerId == dealerId)
                            .ToList();

                         if (filteredOrderItems.Count == 0)
                            continue;

                         order.Products = filteredOrderItems;
                        ordersList.Add(order);
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
                Console.WriteLine($"Error fetching orders: {ex.Message}");
                return new List<Orders>();
            }
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
                var DocumentId = HttpContext.Session.GetString("UserId");

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
            catch  
            {
                 throw new NotImplementedException();
            }
        }
        private decimal CalculateProfitRatio(decimal totalRevenue, decimal totalCost)
        {
            if (totalRevenue == 0)
            {
                return 0;
            }

            decimal profit =  totalCost- totalRevenue;
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
                string dealerId = HttpContext.Session.GetString("UserId");

                CollectionReference ordersCollection = firestoreDb.Collection("orders");
                QuerySnapshot ordersQuery = await ordersCollection.GetSnapshotAsync();

                List<OrdersViewModel> OrdersList = new List<OrdersViewModel>();

                foreach (DocumentSnapshot documentSnapshot in ordersQuery.Documents)
                {
                    try
                    {
                        Orders Order = documentSnapshot.ConvertTo<Orders>();

                        List<OrderItem> filteredOrderItems = Order.Products
                           .Where(item => item.Product != null && item.Product.dealerId == dealerId)
                           .ToList();

                        if (filteredOrderItems.Count == 0)
                            continue;

                        string documentPath = documentSnapshot.Reference.Path;
                        string documentId = documentPath.Split('/').Last();

                        Orders filteredOrder = new Orders
                        {
                            Date = Order.Date ,
                            Address = Order.Address ,
                            OrderId = Order.OrderId ,
                            TotalPrice = Order.TotalPrice ,
                            OrderStatus = Order.OrderStatus ,
                            Products = filteredOrderItems ,
                            CreateTime = Order.CreateTime ,
                            UpdateTime = Order.UpdateTime
                        };

                        OrdersList.Add(new OrdersViewModel { Orders = filteredOrder , DocumentId = documentId });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing document {documentSnapshot.Id}: {ex.Message}");
                    }
                }

                return OrdersList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching orders: {ex.Message}");
                return new List<OrdersViewModel>();
            }
        }
        #endregion
    }
}
