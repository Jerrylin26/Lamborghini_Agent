using System.Diagnostics;
using Lamborghini.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lamborghini.Controllers
{
    public class HelloController : Controller
    {
        private readonly ILogger<HelloController> _logger;

        public HelloController(ILogger<HelloController> logger)
        {
            _logger = logger;
        }


        public IActionResult Index()
        {
            DBmanager dbmanager = new DBmanager();
            List<account> accounts = dbmanager.getAccount();
            ViewBag.accounts = accounts;
            return View();
        }

        public IActionResult addAccount()
        {
            return View();
        }

        [HttpPost]
        public IActionResult addAccount(account user)
        {

            DBmanager dbmanager = new DBmanager();
            try
            {
                dbmanager.newAccount(user);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
