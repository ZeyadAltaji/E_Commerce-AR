using E_CommerceAR.Controllers;
using E_CommerceAR.Domain.ModalsBase;
using E_CommerceAR.Domain.ModalsViews;
using Firebase.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceAR.Areas.DealerAreas.Controllers
{
    [Area("DealerAreas")]

    public class OrdersController : BaseController
    {

        FirebaseAuthProvider auth;
        private readonly FirestoreDb firestoreDb;
        public OrdersController(){
            auth = new FirebaseAuthProvider(
                                      new FirebaseConfig(ApiKey));

            firestoreDb = FirestoreDb.Create(PorjectId);
        }
         [Route("DealerAreas/Orders/Index")]

        public IActionResult Index()
        {
            return View();
        }
         [Route("DealerAreas/Orders/ListOrders")]

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
        private async Task<List<OrdersViewModel>> FetchOrdersFromDatabase ()
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

        //private async Task<List<OrdersViewModel>> FetchOrdersFromDatabase()
        //{
        //    try
        //    {
        //        string dealerId = HttpContext.Session.GetString("UserId");

        //        CollectionReference ordersCollection = firestoreDb.Collection("orders");
        //        QuerySnapshot ordersQuery = await ordersCollection.GetSnapshotAsync();


        //        List<OrdersViewModel> OrdersList = new List<OrdersViewModel>();

        //        foreach (DocumentSnapshot documentSnapshot in ordersQuery.Documents)
        //        {
        //            try
        //            {
        //                //Orders Order = documentSnapshot.ConvertTo<Orders>();
        //                //string documentPath = documentSnapshot.Reference.Path;
        //                //string documentId = documentPath.Split('/').Last();
        //                //OrdersList.Add(new OrdersViewModel { Orders = Order, DocumentId = documentId });
        //                Orders order = documentSnapshot.ConvertTo<Orders>();

        //                // Check if the order belongs to the desired dealer
        //                if (order.Products != null && order.Products.Any())
        //                {
        //                    foreach (OrderItem orderItem in order.Products)
        //                    {
        //                        // Filter out the OrderItem objects where dealerId matches
        //                        if (orderItem.Product != null && orderItem.Product.dealerId == dealerId)
        //                        {
        //                            orderItems.Add(orderItem);
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine($"Error converting document {documentSnapshot.Id}: {ex.Message}");
        //            }
        //        }

        //        return OrdersList;
        //    }
        //    catch (Exception ex)
        //    {
        //         Console.WriteLine($"Error fetching products: {ex.Message}");
        //        return new List<OrdersViewModel>();
        //    }
        //}
        [Route("DealerAreas/Orders/ViewOrder")]

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
