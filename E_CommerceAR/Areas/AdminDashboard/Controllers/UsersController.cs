using E_CommerceAR.Controllers;
using E_CommerceAR.Domain.ModalsBase;
using E_CommerceAR.Domain.ModalsViews;
using Firebase.Auth;
using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceAR.Areas.AdminDashboard.Controllers
{
    [Area("AdminDashboard")]

    public class UsersController : BaseController
    {
        FirebaseAuthProvider auth;
        private readonly FirestoreDb firestoreDb;

        public UsersController()
        {
            auth = new FirebaseAuthProvider(
                    new FirebaseConfig(ApiKey));
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
                "C:\\Users\\ziada\\Source\\repos\\E_CommerceAR\\E_CommerceAR\\Extensions\\" +
                     "finalprojectar-d85ea-firebase-adminsdk-9x4fl-3f47b05b2e.json");
            firestoreDb = FirestoreDb.Create(PorjectId);
        }
        [Route("Users/Index")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("Users/ListUsers")]

        [HttpGet]
        public async Task<IActionResult> ListUsers()
        {
            try
            {
                List<UsersViewModel> UsersDataList = await FetchUserFromDatabase();



                return PartialView(UsersDataList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching orders: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        private async Task<List<UsersViewModel>> FetchUserFromDatabase()
        {
            try
            {
                CollectionReference UsersCollection = firestoreDb.Collection("user");
                QuerySnapshot querySnapshot = await UsersCollection.GetSnapshotAsync();

                List<UsersViewModel> usersList = new List<UsersViewModel>();

                foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
                {
                    try
                    {
                        Users Users = documentSnapshot.ConvertTo<Users>();
                        string documentPath = documentSnapshot.Reference.Path;
                        string documentId = documentPath.Split('/').Last();
                        usersList.Add(new UsersViewModel { Users = Users, DocumentId = documentId });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting document {documentSnapshot.Id}: {ex.Message}");
                    }
                }

                return usersList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching products: {ex.Message}");
                return new List<UsersViewModel>();
            }
        }

        [Route("Users/AddNewUser")]

        public IActionResult AddNewUser()
        {
            return PartialView();
        }
        [Route("Users/EditUser")]
        public IActionResult EditUser(string DocumentId, bool Edit)
        {
            CollectionReference UsersCollection = firestoreDb.Collection("user");

            DocumentReference UsersDocument = UsersCollection.Document(DocumentId);

            DocumentSnapshot snapshot = UsersDocument.GetSnapshotAsync().Result;


            Users product = snapshot.ConvertTo<Users>();
            ViewBag.Edit = Edit;
            ViewBag.DocumentId = DocumentId;

            return PartialView(product);
        }
        [Route("Users/CreateNewUsers")]
         public async Task<IActionResult> CreateNewUsers(Signup SignupModel)
        {
          
                try
                {
                    SignupModel.ISActive = true;
                    SignupModel.ISDeleted = false;
                    SignupModel.Unapproved = false;

                    SignupModel.Role = 2;

                    var user = new Signup
                    {
                        firstName = SignupModel.firstName,
                        lastName = SignupModel.lastName,
                        email = SignupModel.email,
                        password = SignupModel.password,
                        ComfirmPassword = SignupModel.password ,
                        MobileNo = SignupModel.MobileNo ,
                        Role = SignupModel.Role,
                        ISActive = SignupModel.ISActive,
                        ISDeleted = SignupModel.ISDeleted ,
                        Unapproved = SignupModel.Unapproved ,
                        Address = SignupModel.Address ,

                    };
                    await auth
                        .CreateUserWithEmailAndPasswordAsync(SignupModel.email, SignupModel.password);

                    var userCollectionReference = firestoreDb.Collection("user");

                    var documentReference = await userCollectionReference.AddAsync(user);

                    string autoGeneratedDocumentId = documentReference.Id;



                    return RedirectToAction("Index" , "Users");

                }
                catch
                {
                    throw new NotImplementedException();
                }


        }


        [Route("Users/SaveEditUsers")]
        public async Task<IActionResult> SaveEditProducts(Signup SignupModel, string DocumentId)
        {
            try
            {
                var userCollectionReference = firestoreDb.Collection("user");

                var existinguserDocument = await userCollectionReference.Document(DocumentId).GetSnapshotAsync();

                if (existinguserDocument.Exists)
                {
                    var updateData = new Dictionary<string, object>
                    {
                        { "firstName", SignupModel.firstName },
                        { "lastName", SignupModel.lastName },
                        { "email", SignupModel.email },
                        { "password", SignupModel.password },
                    };

                    await userCollectionReference.Document(DocumentId).UpdateAsync(updateData);
                }
                else
                {
                     return RedirectToAction("Index" , "Users");
                }

                return RedirectToAction("Index" , "Users");

            }
            catch
            {
                return RedirectToAction("Index" , "Users");

            }
        }
        [Route("Users/DeleteUsers")]
        [HttpPost]

        public async Task<JsonResult> DeleteUsers(string DocumentId)
        {
            try
            {
                CollectionReference userCollection = firestoreDb.Collection("user");

                DocumentReference userRef = userCollection.Document(DocumentId);

                await userRef.DeleteAsync();

                return Json(new { success = true, message = "user deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting userRef: {ex.Message}" });
            }
        }

    }
}
