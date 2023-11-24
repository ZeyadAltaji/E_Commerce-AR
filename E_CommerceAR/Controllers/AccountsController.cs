using E_CommerceAR.Domain.ModalsViews;
using E_CommerceAR.Extensions;
using Firebase.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using System.Globalization;

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
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "C:\\Users\\ziada\\Source\\repos\\E-commerce AR\\E-CommerceAR.UI\\Extensions\\finalprojectar-d85ea-5769d392d7b0.json");
            firestoreDb = FirestoreDb.Create("FinalProjectAR");

        }
            public IActionResult Login()
            {
                if (HttpContext.Session.GetString("me") == null)
                {
                    return View();
                }
                return RedirectToAction("Login", "Accounts");

            }
            [HttpPost]
            public async Task<IActionResult> Login(Login loginModel)
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

                    HttpContext.Session.SetString("_UserToken", token);

                    var user = await FetchUserFromDatabase(loginModel.Email);
                    HttpContext.Session.SetString("Role", user.Role.ToString());

                    if (user != null)
                    {

                        switch (user.Role)
                        {
                            case 1:
                                return RedirectToAction("Index", "Home", new { area = "AdminDashboard" });

                            case 2:
                                return RedirectToAction("Index", "Home", new { area = "AdminDashboard" });
                            default:
                                return StatusCode(404);

                        }

                    }
                }
                return RedirectToAction("Index", "Home", new { area = "AdminDashboard" });
            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                return View(loginModel);
            }

        }
            private async Task<Login> FetchUserFromDatabase(string email)
            {
                try
                {
                    FirestoreDb db = FirestoreDb.Create("finalprojectar-d85ea");

                    // Assuming "users" is your collection and "Email" is the field you want to search
                    Query query = db.Collection("user").WhereEqualTo("email", email);
                    QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

                    if (querySnapshot.Documents.Count > 0)
                    {
                        // Assuming you have a User class to represent your user data
                        return querySnapshot.Documents[0].ConvertTo<Login>();
                    }

                    return null;

                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            public IActionResult Signup()
            {
                return View();
            }
            [HttpPost]
            public async Task<IActionResult> Signup(Signup SignupModel)
            {
                try
                {
                    SignupModel.IsActive = true;
                    SignupModel.IsDeleted = false;
                    SignupModel.Role = 2;
                    var user = new Signup
                    {
                        firstName = SignupModel.firstName,
                        lastName = SignupModel.lastName,

                        email = SignupModel.email,
                        password = SignupModel.password,
                        Role = SignupModel.Role,
                        IsActive = SignupModel.IsActive,
                        IsDeleted = SignupModel.IsDeleted


                    };

                    ////CollectionReference ColRef = firestoreDb.Collection("user");
                    //await ColRef.AddAsync(user);
                    return RedirectToAction("Login", "Accounts");
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Signup", "Accounts");
                }
            }
        [HttpPost]
        public ActionResult Logout()
        {
            try
            {
                HttpContext.Session.Remove("me");

                // You may want to add additional checks or logic here if needed.

                return Json(new { success = true }); // Return a success response
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
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

                // Set session variable
                HttpContext.Session.SetString("E-CommerceAR_Lang", newLanguage);

                // Set cookie
                Response.Cookies.Append("E-CommerceAR_Lang", newLanguage, new CookieOptions
                {
                    Expires = DateTime.Now.AddYears(10)
                });

                // Set culture
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(newLanguage);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(newLanguage);

                // Set custom date format
                CultureInfo ci = new CultureInfo(newLanguage);
                ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
                Thread.CurrentThread.CurrentCulture = ci;

                return Json(new { ok = "ok", language = newLanguage });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Exception in ChangeLanguage method: {ex.Message}");
                return Json(new { error = "An error occurred while changing the language." });
            }
        }




        [HttpPost]
            private bool IsValidEmail(string email)
            {
                try
                {
                    System.Net.Mail.MailAddress addr = new System.Net.Mail.MailAddress(email);
                    return addr.Address == email;
                }
                catch
                {
                    return false;
                }
            }
        
    }
}
