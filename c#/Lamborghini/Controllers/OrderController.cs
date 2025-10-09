using Microsoft.AspNetCore.Mvc;

namespace Lamborghini.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Checkout()
        {
            return View();
        }
    }
}
