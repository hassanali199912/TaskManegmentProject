using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManegmentProject.Controllers
{
        [AllowAnonymous]
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View("Login");
        }

        public IActionResult Regiester()
        {
            return View("Regiester");
        }
    }
}
