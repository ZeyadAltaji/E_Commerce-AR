using E_CommerceAR.Controllers;
using E_CommerceAR.Domain.ModalsBase;
using E_CommerceAR.Domain.ModalsViews;
using E_CommerceAR.UI.Extensions;
using Firebase.Auth;
using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Net.Mail;

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
            List<ProductViewModel> productDataList = await FetchProductsFromDatabase();

            // Fetch user names for each product concurrently
            var fetchUserTasks = productDataList.Select(product => FetchUserNamesFromDatabase(product.Product.DealerId));
            var userNames = await Task.WhenAll(fetchUserTasks);

            // Add user names to each item in productDataList
            for (int i = 0; i < productDataList.Count; i++)
            {
                productDataList[i].FirstName = userNames[i].FristName;
                productDataList[i].LastName = userNames[i].LastName;
            }

            return PartialView(productDataList);
        }



        private async Task<List<ProductViewModel>> FetchProductsFromDatabase()
        {
            try
            {
                CollectionReference productsCollection = firestoreDb.Collection("Products");
                QuerySnapshot querySnapshot = await productsCollection.GetSnapshotAsync();

                List<ProductViewModel> productList = new List<ProductViewModel>();

                foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
                {
                    try
                    {
                        Products product = documentSnapshot.ConvertTo<Products>();
                        string documentPath = documentSnapshot.Reference.Path;
                        string documentId = documentPath.Split('/').Last();
                        productList.Add(new ProductViewModel { Product = product, DocumentId = documentId });
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
                Console.WriteLine($"Error fetching products: {ex.Message}");
                return new List<ProductViewModel>();
            }
        }

        private async Task<(string FristName, string LastName)> FetchUserNamesFromDatabase(string DealerId)
        {
            try
            {

                DocumentReference docRef = firestoreDb.Collection("user").Document(DealerId);
                DocumentSnapshot documentSnapshot = await docRef.GetSnapshotAsync();

                if (documentSnapshot.Exists)
                {
                    var user = documentSnapshot.ConvertTo<Signup>();
                    return (user.firstName, user.lastName);
                }

                return (null, null);

            }
            catch (Exception ex)
            {
                return (null, null);
            }
        }
         
        [Route("Products/AddNewProduct")]

        public IActionResult AddNewProduct()
        {
            return PartialView();
        }
        [Route("Products/EditProduct")]
        public IActionResult EditProduct(string DocumentId ,bool Edit)
        {
            CollectionReference productsCollection = firestoreDb.Collection("Products");

            DocumentReference productDocument = productsCollection.Document(DocumentId);

            DocumentSnapshot snapshot = productDocument.GetSnapshotAsync().Result;


            Products product = snapshot.ConvertTo<Products>();
            ViewBag.Edit = Edit;
            ViewBag.DocumentId = DocumentId;

            return PartialView(product);
        }
        [Route("Products/SaveNewProducts")]

        public async Task<IActionResult> SaveNewProducts(Products products)
        {
         
                try
                {


                    var addproducts = new Products
                    {
                        Name = products.Name,
                        Category = products.Category,
                        Description = products.Description,
                        Price = products.Price,
                        OfferPercentage = products.OfferPercentage,
                        DealerId = HttpContext.Session.GetString("UserId")



                    };
                   
                    var userCollectionReference = firestoreDb.Collection("Products");

                    var documentReference = await userCollectionReference.AddAsync(addproducts);

                    string autoGeneratedDocumentId = documentReference.Id;
  

                    return RedirectToAction("ListProduct", "Products");
                }
                catch (Exception ex)
                {
                    // Handle the exception appropriately
                    return RedirectToAction("AddNewProduct", "Products");
                }
            
        
    }
        [Route("Products/SaveEditProducts")]
        public async Task<IActionResult> SaveEditProducts(Products products, string DocumentId)
        {
            try
            {
                var userCollectionReference = firestoreDb.Collection("Products");

                 var existingProductDocument = await userCollectionReference.Document(DocumentId).GetSnapshotAsync();

                if (existingProductDocument.Exists)
                {
                     var updateData = new Dictionary<string, object>
            {
                { "name", products.Name },
                { "category", products.Category },
                { "description", products.Description },
                { "price", products.Price },
                { "offerPercentage", products.OfferPercentage },
                { "dealerId", HttpContext.Session.GetString("UserId") }
            };

                    await userCollectionReference.Document(DocumentId).UpdateAsync(updateData);
                }
                else
                {
                     return RedirectToAction("AddNewProduct", "Products");
                }

                return View(Index);
            }
            catch (Exception ex)
            {
                // Handle the exception
                return RedirectToAction("AddNewProduct", "Products");
            }
        }

        [Route("Products/DeleteProduct")]
        [HttpPost]

        public async Task<JsonResult> DeleteProduct(string DocumentId)
        {
            try
            {
                 CollectionReference productsCollection = firestoreDb.Collection("Products");

                 DocumentReference productRef = productsCollection.Document(DocumentId);

                 await productRef.DeleteAsync();

                 return Json(new { success = true, message = "Product deleted successfully." });
            }
            catch (Exception ex)
            {
                 return Json(new { success = false, message = $"Error deleting product: {ex.Message}" });
            }
        }
    }
}
