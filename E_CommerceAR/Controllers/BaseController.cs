using E_CommerceAR.Domain.ModalsBase;
using E_CommerceAR.UI.Extensions;
using Google.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.CodeAnalysis;
using System.Globalization;

namespace E_CommerceAR.Controllers
{
    public class BaseController : Controller
    {
        HttpContext context;
        public readonly static string ApiKey = "AIzaSyBSjwMDM_Cf4STiMVKqCqDXziCvFis3fQU";
        public readonly static string Bucket = "gs://finalprojectar-d85ea.appspot.com/";
        public readonly static string PorjectId = "finalprojectar-d85ea";
        private static string documentId;

        public string Title { get; set; }
            private string Lang;
 
            public string Language
            {
                get
                {
                    if (string.IsNullOrEmpty(HttpContext.Session.GetString("E-CommerceAR_Lang")))
                    {
                        Lang = "en";
                    }
                    return Lang;
                }
            }
            private Users user;
            public Users me
            {
                get
                {
                    if (HttpContext.Session.GetString("_UserToken") != null)
                    {
                        HttpContext.Session.SetObject("_UserToken", user);
                    }
                    return user;
                }
            }
        public string DocumentId
        {
            get { return documentId; }
            set { documentId = context.Session.GetString("UserId"); }
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                string CuurentURL = filterContext.Controller.ToString();
                string originAction = filterContext.RouteData.Values["action"].ToString();

                string theme = Request.Cookies["Theme"];
                string langName = Request.Cookies["E-CommerceAR_Lang"];

                if (HttpContext.Session.GetString("Theme") == null)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(theme))
                        {
                            HttpContext.Session.SetString("Theme", "sidebar-mini skin-purple");
                        }
                        else
                        {
                            HttpContext.Session.SetString("Theme", theme);
                        }
                    }
                    catch (Exception)
                    {
                        HttpContext.Session.SetString("Theme", "sidebar-mini skin-purple");
                    }
                }

                if (HttpContext.Session.GetString("E-CommerceAR_Lang") == null)
                {
                    langName = "ar-Pl";
                    HttpContext.Session.SetString("E-CommerceAR_Lang", "ar-Pl");
                    Response.Cookies.Append("E-CommerceAR_Lang", "ar-Pl", new CookieOptions
                    {
                        Expires = DateTime.Now.AddYears(10)
                    });
                }
                else
                {
                    langName = HttpContext.Session.GetString("E-CommerceAR_Lang");
                    Response.Cookies.Append("E-CommerceAR_Lang", langName, new CookieOptions
                    {
                        Expires = DateTime.Now.AddYears(10)
                    });
                }

                ViewBag.ID = langName;
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(langName);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(langName);
                CultureInfo ci = new CultureInfo(langName);
                ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
                Thread.CurrentThread.CurrentCulture = ci;

                if (!filterContext.Controller.ToString().Contains("AccountsController"))
                {
                    filterContext.HttpContext.Response.Headers.Add("Cache-Control", "no-store");
                    if (HttpContext.Session.GetString("_UserToken") == null)
                    {
                        filterContext.Result = new RedirectResult(Url.Action("Logout", "Account", new { area = string.Empty }));
                    }
                }
            }
            public string Translate(string Ar, string En)
            {
                if (HttpContext.Session.GetString("E-CommerceAR_Lang") == "ar-JO")
                {
                    return Ar;
                }
                else
                {
                    return En;
                }
            }
        }
}
