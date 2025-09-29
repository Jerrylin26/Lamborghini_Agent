using Microsoft.AspNetCore.Mvc;

namespace Lamborghini.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

    }
}
