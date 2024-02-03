using E_CommerceAR.Domain.ModalsBase;
using E_CommerceAR.Extensions;
using Firebase.Auth;
using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.RegularExpressions;

namespace E_CommerceAR.Controllers
{
    public class AccountsController : BaseController
    {

        FirebaseAuthProvider auth;
        private readonly FirestoreDb firestoreDb;

        public AccountsController()
        {
            auth = new FirebaseAuthProvider(
                        new FirebaseConfig(ApiKey));
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", 
                "C:\\Users\\ziada\\Source\\repos\\E_CommerceAR\\E_CommerceAR\\Extensions\\" +
                     "finalprojectar-d85ea-firebase-adminsdk-9x4fl-3f47b05b2e.json");
            firestoreDb = FirestoreDb.Create(PorjectId);

        }
        public IActionResult Login()
        {
            string _UserToken = HttpContext.Session.GetString("_UserToken");

            if (_UserToken == null)
            {
                return View();
            }

            string Role = HttpContext.Session.GetString("Role");

            if (int.TryParse(Role, out int roleId))
            {
                return roleId switch
                {
                    1 => RedirectToAction("Index", "Home", new { area = "AdminDashboard" }),
                    2 => RedirectToAction("Index", "Home", new { area = "DealerAreas" }),
                    _ => StatusCode(404)
                };
            }

            return StatusCode(404);
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login loginModel)
        {
 
            if (ModelState.IsValid)
            {
                try
                {

                    if (!IsValidEmail(loginModel.Email))
                    {
                        ModelState.AddModelError("Email", "Invalid email address");
                        return View(loginModel);
                    }

                    var fbAuthLink = await auth.SignInWithEmailAndPasswordAsync(loginModel.Email, loginModel.Password);
                    string token = fbAuthLink.FirebaseToken;
                    if (token != null)
                    {


                        var (user, documentId) = await FetchUserFromDatabase(loginModel.Email);

                        if (user != null)
                        {
                            if (!user.IsActive)
                            {
                                ViewData["IsActive"] = "Please wait for the official's approval.";
                                return View(loginModel);
                            }

                            if (user.IsDeleted == true)
                            {
                                ViewData["IsDeleted"] = "Your account is deleted. Please contact the administrator.";
                                return View(loginModel);
                            }
                            HttpContext.Session.SetString("_UserToken", token);
                            HttpContext.Session.SetString("Role", user.Role.ToString());
                            HttpContext.Session.SetString("UserId", documentId);


                            switch (user.Role)
                            {
                                case 1:
                                    return RedirectToAction("Index", "Home", new { area = "AdminDashboard" });

                                case 2:
                                    return RedirectToAction("Index", "Home", new { area = "DealerAreas" });
                                default:
                                    return StatusCode(404);

                            }

                        }
                    }
                    ViewData["InvalidMessage"] = Translate("بريد إلكتروني أو كلمة مرور غير صالحة", "Invalid Email or Password");
                    return View(loginModel);
                }
                catch (FirebaseAuthException ex)
                {
                    var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                    ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                    ViewData["InvalidMessage"] = Translate("بريد إلكتروني أو كلمة مرور غير صالحة", "Invalid Email or Password");

                    return View(loginModel);
                }
            }
            else
            {
                ViewData["ErrorMessage"] = Translate("هذا الحقل الزامي", "This field is required");
                return View(loginModel);

            }

        }
        private async Task<(Login User, string DocumentId)> FetchUserFromDatabase(string email)
        {
            try
            {
                Query query = firestoreDb.Collection("user").WhereEqualTo("email", email);
                QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

                if (querySnapshot.Documents.Count > 0)
                {
                    string documentPath = querySnapshot.Documents[0].Reference.Path;
                    string documentId = documentPath.Split('/').Last();
                    return (querySnapshot.Documents[0].ConvertTo<Login>(), documentId);
                }

                return (null, null);
            }
            catch
            {
                return (null, null);
            }
        }

        public IActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Signup(Signup SignupModel, IFormFile attachments)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    
                    SignupModel.ISActive = false;
                    SignupModel.ISDeleted = false;
                    SignupModel.Unapproved = true;

                    SignupModel.Role = 2;

                    var user = new Signup
                    {
                        firstName = SignupModel.firstName,
                        lastName = SignupModel.lastName,
                        email = SignupModel.email,
                        password = SignupModel.password,
                        ComfirmPassword = SignupModel.ComfirmPassword ,
                        MobileNo = SignupModel.MobileNo ,

                        Role = SignupModel.Role,
                        ISActive = SignupModel.ISActive,
                        ISDeleted = SignupModel.ISDeleted,
                        Unapproved= SignupModel.Unapproved,
                        Address = SignupModel.Address ,


                    };
                    await auth
                        .CreateUserWithEmailAndPasswordAsync(SignupModel.email, SignupModel.password);

                    var userCollectionReference = firestoreDb.Collection("user");

                    var documentReference = await userCollectionReference.AddAsync(user);

                    string autoGeneratedDocumentId = documentReference.Id;
                    string folderName = $"Attachments/{autoGeneratedDocumentId}/";

                    if (attachments != null && attachments.Length > 0)
                    {
                        string bucketName = "finalprojectar-d85ea.appspot.com";
                        var storage = StorageClient.Create();

                        string fileName = $"{folderName}{attachments.FileName}";

                        using (var stream = attachments.OpenReadStream())
                        {
                            storage.UploadObject(bucketName, fileName, null, stream);
                        }
                    }

                    return RedirectToAction("Login", "Accounts");
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("EMAIL_EXISTS"))
                    {
                        ViewData["EMAIL_EXISTS"] = "The email address is already in use.";
                    }
                   else if (ex.Message.Contains("WEAK_PASSWORD"))
                    {
                        ViewData["WEAK_PASSWORD"] = "The password is Weak .";
                    }
                    else
                    {
                         ViewData["ErrorMessage"] = "An error occurred while creating the account.";
                    }         
                    return RedirectToAction("Signup", "Accounts");
                }
            }
            else
            {
                if (attachments == null)
                {
                    ViewData["ErrorMessage"] = Translate("هذا الحقل الزامي", "This field is required");


                }
               else if (SignupModel.password != SignupModel.ComfirmPassword)
               {
                    ViewData["ErrorMessageConfirmPassword"] = "The password and confirmation password do not match.";
               }
                ViewData["ErrorMessage"] = Translate("هذا الحقل الزامي", "This field is required");

                return View("Signup", SignupModel);
            }
        }

        [HttpPost]
        public ActionResult Logout()
        {
            try
            {
                HttpContext.Session.Remove("_UserToken");


                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error during logout: {ex.Message}");
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ChangeLanguage()
        {
            try
            {
                string currentLanguage = HttpContext.Session.GetString("E-CommerceAR_Lang");
                string newLanguage = "";

                if (currentLanguage == "en")
                {
                    newLanguage = "ar-Pl";
                }
                else
                {
                    newLanguage = "en";
                }

                HttpContext.Session.SetString("E-CommerceAR_Lang", newLanguage);

                Response.Cookies.Append("E-CommerceAR_Lang", newLanguage, new CookieOptions
                {
                    Expires = DateTime.Now.AddYears(10)
                });

                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(newLanguage);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(newLanguage);

                CultureInfo ci = new CultureInfo(newLanguage);
                ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
                Thread.CurrentThread.CurrentCulture = ci;

                return Json(new { ok = "ok", language = newLanguage });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in ChangeLanguage method: {ex.Message}");
                return Json(new { error = "An error occurred while changing the language." });
            }
        }




        [HttpPost]
        public bool IsValidEmail(string email)
        {
            try
            {
                 string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

                 if (Regex.IsMatch(email , pattern))
                 {
                        return true;
                 }
                else
                {
                     return false;
                }
            }
            catch
            {
                 return false;
            }
        }

    }
}
