using E_CommerceAR.Controllers;
 using E_CommerceAR.Domain.ModalsBase;
using E_CommerceAR.Domain.ModalsViews;
using Firebase.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
  using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Reactive.Subjects;

namespace E_CommerceAR.Areas.AdminDashboard.Controllers
{
    [Area("AdminDashboard")]
    public class UserApprovalController : BaseController
    {
        FirebaseAuthProvider auth;
        private readonly FirestoreDb firestoreDb;
        private readonly StorageClient storageClient = StorageClient.Create();
        private readonly IConfiguration _configuration;

        public UserApprovalController(IConfiguration Configuration)
        {
            auth = new FirebaseAuthProvider(
                    new FirebaseConfig(ApiKey));
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
                "C:\\Users\\ziada\\Source\\repos\\E_CommerceAR\\E_CommerceAR\\Extensions\\" +
                     "finalprojectar-d85ea-firebase-adminsdk-9x4fl-3f47b05b2e.json");
            firestoreDb = FirestoreDb.Create(PorjectId);
            _configuration = Configuration;

        }
        [Route("UserApproval/Index")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("UserApproval/ListUnapprovedUsers")]

        [HttpGet]
        public async Task<IActionResult> ListUnapprovedUsers()
        {
            try
            {
                List<AttachmentViewModel> unapprovedUsersList = new List<AttachmentViewModel>();

                List<UsersViewModel> usersList = await FetchUnapprovedUserFromDatabase();

                foreach (UsersViewModel userViewModel in usersList)
                {
                    List<AttachmentViewModel> attachmentsList = await FetchAttachmentsFromStorage(userViewModel.DocumentId);

                    AttachmentViewModel unapprovedUserViewModel = new AttachmentViewModel
                    {
                        UsersViewModel = userViewModel,
                        AttachmentId = attachmentsList.FirstOrDefault()?.AttachmentId,
                        FileName = attachmentsList.FirstOrDefault()?.FileName
                    };

                    unapprovedUsersList.Add(unapprovedUserViewModel);
                }

                return PartialView(unapprovedUsersList);
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Error fetching unapproved users: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        private async Task<List<UsersViewModel>> FetchUnapprovedUserFromDatabase()
        {
            try
            {
                CollectionReference UsersCollection = firestoreDb.Collection("user");
                QuerySnapshot querySnapshot = await UsersCollection
                .WhereEqualTo("ISActive", false).WhereEqualTo("Unapproved" , true)

                    .GetSnapshotAsync();

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
                // Handle exception
                Console.WriteLine($"Error fetching inactive users: {ex.Message}");
                return new List<UsersViewModel>();
            }
        }
        public async Task<List<AttachmentViewModel>> FetchAttachmentsFromStorage(string documentId)
        {
            try
            {
                string folderPath = $"Attachments/{documentId}/";
                string bucketName = "finalprojectar-d85ea.appspot.com";

                var storageObjects = await storageClient.ListObjectsAsync(bucketName, folderPath).ToListAsync();

                List<AttachmentViewModel> attachmentsList = new List<AttachmentViewModel>();

                foreach (var storageObject in storageObjects)
                {
                    try
                    {
                        attachmentsList.Add(new AttachmentViewModel
                        {
                            AttachmentId = storageObject.Name,
                            FileName = storageObject.Name.Split('/').Last()
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing attachment file {storageObject.Name}: {ex.Message}");
                    }
                }

                return attachmentsList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching attachments from storage for user {documentId}: {ex.Message}");
                return new List<AttachmentViewModel>();
            }
        }
        [Route("UserApproval/UnapprovedUser")]

        public IActionResult UnapprovedUser(string DocumentId, string Full_Name, string Email)
        {
            ViewBag.DocumentId= DocumentId;
            ViewBag.Full_Name= Full_Name;
            ViewBag.Email= Email;
            return PartialView();
        }
        [Route("UserApproval/NoneUnapprovedUser")]

        public IActionResult NoneUnapprovedUser(string DocumentId, string Full_Name, string Email)
        {
            ViewBag.DocumentId = DocumentId;
            ViewBag.Full_Name = Full_Name;
            ViewBag.Email = Email;
            return PartialView();
        }
        [Route("UserApproval/SendUnapprovedUser")]

        public async Task<IActionResult> SendUnapprovedUser(string DocumentId)
        {
            try
            {
 
                var userCollectionReference = firestoreDb.Collection("user");

                var existingProductDocument = await userCollectionReference.Document(DocumentId).GetSnapshotAsync();

                var updateData = new Dictionary<string, object>
            {
                { "ISActive", true },
                { "Unapproved", true}

            };

                await userCollectionReference.Document(DocumentId).UpdateAsync(updateData);


                return View("Index");
            }
            catch (Exception ex)
            {
                // Handle the exception
                return RedirectToAction("UnapprovedUser", "UserApproval");
            }
        }
        [Route("UserApproval/SendNoneUnapprovedUser")]

        public async Task<IActionResult> SendNoneUnapprovedUser(string DocumentId)
        {
            try
            {
                 
                var userCollectionReference = firestoreDb.Collection("user");

                var existingProductDocument = await userCollectionReference.Document(DocumentId).GetSnapshotAsync();

                var updateData = new Dictionary<string, object>
            {
                { "ISActive", false },
                { "Unapproved", false }
            };

                await userCollectionReference.Document(DocumentId).UpdateAsync(updateData);


                return View("Index");
            }
            catch (Exception ex)
            {
                // Handle the exception
                return RedirectToAction("Unapproved", "Unapproved");
            }
        }
        
    }

       
}
