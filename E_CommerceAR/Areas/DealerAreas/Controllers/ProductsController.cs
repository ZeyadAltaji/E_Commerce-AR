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

    public class ProductsController : BaseController
    {

        FirebaseAuthProvider auth;
        private readonly FirestoreDb firestoreDb;
        public ProductsController()
        {
            auth = new FirebaseAuthProvider(
                                      new FirebaseConfig(ApiKey));

            firestoreDb = FirestoreDb.Create(PorjectId);
        }
        [Route("DealerAreas/Products/Index")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [Route("DealerAreas/Products/ListProduct")]
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
                Query productsCollection = firestoreDb.Collection("Products").WhereEqualTo("dealerId", DocumentId);
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

         [Route("DealerAreas/Products/AddNewProduct")]
        public IActionResult AddNewProduct()
        {
            return PartialView();
        }
         [Route("DealerAreas/Products/EditProduct")]

        public IActionResult EditProduct(string DocumentId, bool Edit)
        {
            CollectionReference productsCollection = firestoreDb.Collection("Products");

            DocumentReference productDocument = productsCollection.Document(DocumentId);

            DocumentSnapshot snapshot = productDocument.GetSnapshotAsync().Result;


            Products product = snapshot.ConvertTo<Products>();
            ViewBag.Edit = Edit;
            ViewBag.DocumentId = DocumentId;

            return PartialView(product);
        }
         [Route("DealerAreas/Products/SaveNewProducts")]

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
                    DealerId = DocumentId



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
         [Route("DealerAreas/Products/SaveEditProducts")]

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

                  [Route("DealerAreas/Products/DeleteProduct")]

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
