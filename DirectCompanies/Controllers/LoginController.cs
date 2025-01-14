
using Microsoft.AspNetCore.Mvc;


namespace DirectCompanies.Controllers
{
    [Route("[controller]/[action]")]
    public class LoginController : Controller
    {
        public IActionResult SetLoginCookies(string UserName)
        {
          
            if (UserName != null)
            {
                HttpContext.Response.Cookies.Append("UserName", UserName,
                    new CookieOptions
                    {
                        Expires= DateTimeOffset.UtcNow.AddHours(24)
                    }
                    );
            }
            return LocalRedirect( "/");
        }
    }
}