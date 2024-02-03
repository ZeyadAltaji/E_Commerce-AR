using E_CommerceAR.Controllers;
using E_CommerceAR.Domain.ModalsBase;
using E_CommerceAR.Domain.ModalsViews;
using Firebase.Auth;
using Firebase.Storage;
using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Web;

namespace E_CommerceAR.Areas.DealerAreas.Controllers
{
    [Area("DealerAreas")]

    public class ProductsController : BaseController
    {

        FirebaseAuthProvider auth;
        private readonly FirestoreDb firestoreDb;
        public ProductsController ()
        {
            auth = new FirebaseAuthProvider(
                    new FirebaseConfig(ApiKey));
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS" ,
                "C:\\Users\\ziada\\Source\\repos\\E_CommerceAR\\E_CommerceAR\\Extensions\\" +
                     "finalprojectar-d85ea-firebase-adminsdk-9x4fl-3f47b05b2e.json");
            firestoreDb = FirestoreDb.Create(PorjectId);
        }
        [Route("DealerAreas/Products/Index")]
        [HttpGet]
        public IActionResult Index()
        {
            HttpContext.Session.Remove("Upload");

            return View();
        }
        [Route("DealerAreas/Products/ListProduct")]
        [HttpGet]
        public async Task<IActionResult> ListProduct()
        {
            HttpContext.Session.Remove("Upload");

            List<ProductViewModel> productDataList = await FetchProductsFromDatabase();

             var fetchUserTasks = productDataList.Select(product => FetchUserNamesFromDatabase(product.Product.DealerId));
            var userNames = await Task.WhenAll(fetchUserTasks);

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
                var DocumentId = HttpContext.Session.GetString("UserId");
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
            catch
            {
                return (null, null);
            }
        }

         [Route("DealerAreas/Products/AddNewProduct")]
        public IActionResult AddNewProduct()
        {
            HttpContext.Session.Remove("Upload");

            string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory() , "wwwroot");
            string jsonFilePath = Path.Combine(wwwrootPath , "Colors" , "colors.json");

            string jsonContent = System.IO.File.ReadAllText(jsonFilePath);

            List<Colorslist> colorsList = JsonConvert.DeserializeObject<List<Colorslist>>(jsonContent);
            string jsonSizeList = Path.Combine(wwwrootPath , "Size" , "SizeList.json");

            string jsonContentSizeList = System.IO.File.ReadAllText(jsonSizeList);

            List<SizeList> SizeList = JsonConvert.DeserializeObject<List<SizeList>>(jsonContentSizeList);
            ViewBag.ColorsList = colorsList;
            ViewBag.SizeList = SizeList;
            return PartialView();
        }
        [Route("DealerAreas/Products/ShowFileProduct")]

        public IActionResult ShowFileProduct ()
        {
            List<Upload> uol = JsonConvert.DeserializeObject<List<Upload>>(HttpContext.Session.GetString("Upload"));
            return PartialView(uol);
        }
        [Route("DealerAreas/Products/EditProduct")]

        public IActionResult EditProduct(string DocumentId, bool Edit)
        {
            CollectionReference productsCollection = firestoreDb.Collection("Products");

            DocumentReference productDocument = productsCollection.Document(DocumentId);

            DocumentSnapshot snapshot = productDocument.GetSnapshotAsync().Result;
            string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory() , "wwwroot");
            string jsonFilePath = Path.Combine(wwwrootPath , "Colors" , "colors.json");

            string jsonContent = System.IO.File.ReadAllText(jsonFilePath);

            List<Colorslist> colorsList = JsonConvert.DeserializeObject<List<Colorslist>>(jsonContent);
            string jsonSizeList = Path.Combine(wwwrootPath , "Size" , "SizeList.json");

            string jsonContentSizeList = System.IO.File.ReadAllText(jsonSizeList);

            List<SizeList> SizeList = JsonConvert.DeserializeObject<List<SizeList>>(jsonContentSizeList);
            ViewBag.ColorsList = colorsList;
            ViewBag.SizeList = SizeList;

            Products product = snapshot.ConvertTo<Products>();
            ViewBag.Edit = Edit;
            ViewBag.DocumentId = DocumentId;

            return PartialView(product);
        }
        [Route("DealerAreas/Products/eShowFileProduct")]

        public async Task<IActionResult> eShowFileProduct (string DocumentId , bool Edit)
        {
            List<Upload> uol = new List<Upload>();
            CollectionReference productsCollection = firestoreDb.Collection("Products");

            DocumentReference productDocument = productsCollection.Document(DocumentId);

            DocumentSnapshot snapshot = productDocument.GetSnapshotAsync().Result;

            Products product = snapshot.ConvertTo<Products>();
            ViewBag.Edit = Edit;
            ViewBag.DocumentId = DocumentId;
            ViewBag.Id = product.Id;
            if (product.images.Count != 0)
            {
                uol = await ConvertImagesToUploadList(product.images , product.Id);
            }
            if ((string)HttpContext.Session.GetString("Upload") != null)
            {
                uol = JsonConvert.DeserializeObject<List<Upload>>(HttpContext.Session.GetString("Upload"));
                uol = uol.Where(x => x.ProductId == product.Id).ToList();
            }

            HttpContext.Session.SetString("Upload" , JsonConvert.SerializeObject(uol));

            return PartialView(uol);
        }
        public async Task<List<Upload>> ConvertImagesToUploadList (List<string> imageUrls , string Id)
        {
            try
            {
                List<Upload> uploadList = new List<Upload>();

                using (HttpClient client = new HttpClient())
                {
                    foreach (var imageUrl in imageUrls)
                    {
                        string decodedImageUrl = HttpUtility.UrlDecode(imageUrl);
                        string[] urlParts = decodedImageUrl.Split('?');
                        string imageNameWithExtension = urlParts[0];

                        string[] namePartss = imageNameWithExtension.Split('/');
                        string imageName = namePartss[namePartss.Length - 1];

                        string[] nameParts = imageName.Split('.');
                        string formatType = nameParts[nameParts.Length - 1];
                        formatType = formatType.Split('?')[0];


                        byte[] imageBytes = await client.GetByteArrayAsync(imageUrl);

                        string base64 = Convert.ToBase64String(imageBytes);

                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageName);

                        Upload upload = new Upload
                        {
                            ProductId = Id ,
                            Id = uploadList.Count + 1 ,
                            Name = fileNameWithoutExtension ,
                            ext = formatType ,
                            base64 = base64 ,
                            linkDB = imageUrl ,
                            ImageName = imageName ,
                        };

                        uploadList.Add(upload);
                    }
                }

                return uploadList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing existing files: {ex.Message}");
                return null;
            }
        }
        [Route("DealerAreas/Products/SaveNewProducts")]
        public async Task<IActionResult> SaveNewProducts (string Name , string Category , string Description , double Price , double OfferPercentage , IFormFile Model , List<IFormFile> images , List<string> sizes)
        {
            try
            {
                string autoGeneratedId = Guid.NewGuid().ToString();
                string id = Guid.NewGuid().ToString();

                string folderName = $"products/images/{id}";
                string bucketName = "finalprojectar-d85ea.appspot.com";
                var imageUrl = await UploadImageToFirebaseStorage(Model , folderName , Model.FileName , bucketName);

                var addproducts = new Products
                {
                    Id = autoGeneratedId ,
                    Name = Name ,
                    Category = Category ,
                    Description = Description ,
                    Price = Price ,
                    OfferPercentage = OfferPercentage ,
                    Model = imageUrl ,
                    DealerId = HttpContext.Session.GetString("UserId") ,
                    sizes = sizes.ToList() ,
                    Attachments_Id = id.ToString()
                };

                var userCollectionReference = firestoreDb.Collection("Products");

                if (images.Count != 0)
                {
                    List<string> imagesUrls = new List<string>();

                    foreach (var image in images)
                    {
                        var objectName = $"products/images/{id}";

                        using (MemoryStream stream = new MemoryStream())
                        {
                            await image.CopyToAsync(stream);

                            stream.Position = 0;
                            var firebaseStorage = new FirebaseStorage(bucketName , new FirebaseStorageOptions { ThrowOnCancel = true });
                            var task = firebaseStorage.Child(objectName).Child(image.FileName).PutAsync(stream);

                            var downloadUrl = await task;

                            imagesUrls.Add(downloadUrl);
                        }
                    }

                    addproducts.images = imagesUrls;
                }

                var documentReference = await userCollectionReference.AddAsync(addproducts);

                string autoGeneratedDocumentId = documentReference.Id;

                return RedirectToAction("ListProduct" , "Products");
            }
            catch 
            {
                 return RedirectToAction("AddNewProduct" , "Products");
            }
        }

        private async Task<string> UploadImageToFirebaseStorage (IFormFile file , string folderName , string fileName , string bucketName)
        {
            var storage = new FirebaseStorage(bucketName);

            using (var stream = file.OpenReadStream())
            {
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    memoryStream.Seek(0 , SeekOrigin.Begin);

                    var imageUrl = await storage.Child(folderName)
                       .Child(fileName)
                       .PutAsync(memoryStream);

                    return imageUrl;
                }
            }
        }
        [Route("DealerAreas/Products/SaveEditProducts")]

        public async Task<IActionResult> SaveEditProducts (List<IFormFile> images , string id , string Name , string Category , string Description , double Price , double OfferPercentage , IFormFile Model , List<string> sizes , string DocumentId)
        {
            try
            {
                var userCollectionReference = firestoreDb.Collection("Products");
                string folderName = $"products/images/{id}";
                string bucketName = "finalprojectar-d85ea.appspot.com";
                var existingProductDocument = await userCollectionReference.Document(DocumentId).GetSnapshotAsync();
                var imageUrl = await UploadImageToFirebaseStorage(Model , folderName , Model.FileName , bucketName);
                await RemoveExistingFiles(folderName , bucketName);

                if (existingProductDocument.Exists)
                {

                    var updateData = new Products
                    {
                        Id = id ,
                        Name = Name ,
                        Category = Category ,
                        Description = Description ,
                        Price = Price ,
                        OfferPercentage = OfferPercentage ,
                        Model = imageUrl ,
                        DealerId = HttpContext.Session.GetString("UserId") ,
                        sizes = sizes.ToList() ,
                        Attachments_Id = id.ToString()
                    };
                    if (images.Count != 0)
                    {
                        List<string> imagesUrls = new List<string>();

                        foreach (var image in images)
                        {
                            var objectName = $"products/images/{id}";

                            using (MemoryStream stream = new MemoryStream())
                            {
                                await image.CopyToAsync(stream);

                                stream.Position = 0;
                                var firebaseStorage = new FirebaseStorage(bucketName , new FirebaseStorageOptions { ThrowOnCancel = true });
                                var task = firebaseStorage.Child(objectName).Child(image.FileName).PutAsync(stream);

                                var downloadUrl = await task;

                                imagesUrls.Add(downloadUrl);
                            }
                        }

                        updateData.images = imagesUrls;
                    }


                    await userCollectionReference.Document(DocumentId).SetAsync(updateData);

                }
                else
                {
                    return RedirectToAction("EditProduct" , "Products");
                }

                return View(Index);
            }
            catch 
            {
                 return RedirectToAction("EditProduct" , "Products");
            }
        }
        private async Task RemoveExistingFiles (string folderName , string bucketName)
        {
            try
            {
                var storage = StorageClient.Create();
                var bucket = storage.GetBucket(bucketName);

                var objects = storage.ListObjects(bucketName , folderName);

                foreach (var obj in objects)
                {
                    storage.DeleteObject(bucketName , obj.Name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing existing files: {ex.Message}");
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

                return Json(new { success = true , message = "Product deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false , message = $"Error deleting product: {ex.Message}" });
            }
        }
        [Route("DealerAreas/Products/ArchiveFile")]

        public JsonResult ArchiveFile ()
        {

            List<Upload> uol = new List<Upload>();
            var ProductId = Request.Form["ProductId"];

            int i = 1;
            if ((string)HttpContext.Session.GetString("Upload") == null)
            {
                if (Request.Form.Files.Count > 0)
                {

                    foreach (IFormFile file in Request.Form.Files)
                    {
                        Upload b = new Upload();
                        var _file = file.OpenReadStream();
                        BinaryReader rdr = new BinaryReader(_file);
                        byte[] FileByte = rdr.ReadBytes((int)file.Length);
                        b.Id = i;
                        b.ProductId = ProductId;
                        b.ext = file.FileName.Split('.')[1];
                        b.Name = file.FileName;
                        b.ContentType = file.ContentType;
                        b.base64 = Convert.ToBase64String(FileByte);

                        uol.Add(b);
                        i++;
                    }
                }
            }
            else
            {
                uol = JsonConvert.DeserializeObject<List<Upload>>(HttpContext.Session.GetString("Upload"));
                i = uol.Count() + 1;
                if (Request.Form.Files.Count > 0)
                {
                    foreach (IFormFile file in Request.Form.Files)
                    {
                        Upload s = uol.Where(x => x.Name == file.FileName && x.ProductId == ProductId).FirstOrDefault();
                        if (s == null)
                        {
                            Upload b = new Upload();
                            var _file = file.OpenReadStream();
                            BinaryReader rdr = new BinaryReader(_file);
                            byte[] FileByte = rdr.ReadBytes((int)file.Length);
                            b.Id = i;
                            b.ProductId = ProductId;
                            b.ext = file.FileName.Split('.')[1];
                            b.Name = file.FileName;
                            b.ContentType = file.ContentType;
                            b.base64 = Convert.ToBase64String(FileByte);
                            uol.Add(b);
                            i++;
                        }
                    }
                }
            }
            string Upload = JsonConvert.SerializeObject(uol);
            HttpContext.Session.SetString("Upload" , Upload);
            return Json("Ok");
        }
        [Route("DealerAreas/Products/DeleteFile")]

        public JsonResult DeleteFile (int id , string ProductId)
        {
            List<Upload> uol = new List<Upload>();
            uol = JsonConvert.DeserializeObject<List<Upload>>(HttpContext.Session.GetString("Upload"));
            if (ProductId == null)
            {
                Upload s = uol.Where(x => x.Id == id).FirstOrDefault();
                if (s != null)
                {
                    uol.Remove(s);

                }
            }
            else if (ProductId != null)
            {
                Upload s = uol.Where(x => x.Id == id && x.ProductId == ProductId).FirstOrDefault();
                if (s != null)
                {
                    uol.Remove(s);

                }

            
            }

            string Upload = JsonConvert.SerializeObject(uol);
            HttpContext.Session.SetString("Upload" , Upload);
            return Json("Ok");
        }
    }
}

