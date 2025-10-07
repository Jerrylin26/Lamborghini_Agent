using Lamborghini.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace Lamborghini.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddToCookie(string Series, string Model, int PID) // 選這兩變數原因, 配合DBmanager.cs 的 getDetail(string Series, string Model) // 多餘變數, 但先保留
        {
            List<CartItem> cart;

            // 取得 cookie資訊
            if (Request.Cookies.TryGetValue("LamboCart", out var cookieValue))
            {
                cart = JsonSerializer.Deserialize<List<CartItem>>(cookieValue) ?? new List<CartItem>();
            }
            else
            {
                cart = new List<CartItem>();
            }

            // 找出是否已存在該商品
            var existItem = cart.FirstOrDefault(c => c.ProductId == PID);

            if (existItem != null)
            {
                // 已存在,數量加1
                existItem.Quantity += 1;
            }
            else
            {
                // 不存在,新增商品
                cart.Add(new CartItem { ProductId = PID, Quantity = 1 });
            }

            //DBmanager db = new DBmanager();
            //Debug.WriteLine($"Series={Series}, Model={Model}, PID={PID}");

            //List<Product> cars = db.getDetail(Series, Model); // 多張img 同車款資料
            //Product car = cars[0]; // 取第一張img 當作該車款代表



            // 存回 Cookie
            Response.Cookies.Append("LamboCart", JsonSerializer.Serialize(cart), new CookieOptions // 將選擇的車款PID 存入cookie
            {
                HttpOnly = true,
                Secure = true,       // HTTPS
                Expires = DateTimeOffset.UtcNow.AddDays(0.04)
            });

            // 計算總數量
            int totalQuantity = cart.Sum(c => c.Quantity);

            return Json(new { success = true, totalQuantity = totalQuantity });



        }

        public IActionResult Remove()
        {
            return View();
        }

        public IActionResult Update()
        {
            return View();
        }
    }
}
