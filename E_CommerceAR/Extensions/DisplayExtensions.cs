using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using E_CommerceAR.Domain.ModalsViews;
namespace E_CommerceAR.Extensions
{
    public static class DisplayExtensions
    {
        public static HtmlString Translate(this IHtmlHelper html, string Arabic, string English)
        {
            var httpContextAccessor = html.ViewContext.HttpContext.RequestServices.GetService<IHttpContextAccessor>();

            if (httpContextAccessor != null)
            {
                string language = (string)httpContextAccessor.HttpContext.Session.GetString("E-CommerceAR_Lang");

                if (language == "en")
                {
                    return new HtmlString(English);
                }
                else
                {
                    return new HtmlString(Arabic);
                }
            }
            else
            {
                throw new InvalidOperationException("IHttpContextAccessor is null.");
            }
        }
    }
}
