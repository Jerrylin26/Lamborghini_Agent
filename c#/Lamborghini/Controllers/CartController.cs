using Lamborghini.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

namespace Lamborghini.Controllers
{
    public class CartController : Controller
    {

        /*-------------------------------------------------------------------------------------------------------*/
        // 購物車內商品數量 + - 功能 (AJAX) 著重在 cookie邏輯

        // Cart/AddSpecCart?PID=
        public IActionResult AddSpecCart(int PID) {
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

            // 找出該商品
            var existItem = cart.FirstOrDefault(c => c.ProductId == PID);
            existItem.Quantity += 1; // 關鍵邏輯

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

        // Cart/MinusSpecCart?PID=
        public IActionResult MinusSpecCart(int PID)
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

            // 找出該商品
            var existItem = cart.FirstOrDefault(c => c.ProductId == PID);
            if (existItem.Quantity <= 1)
            {
                return Json(new { success = false, message = "剩 數量:1, 只能刪除!!!" });
            }
            existItem.Quantity -= 1; // 關鍵邏輯

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

        // Cart/DeleteSpecCart?PID=
        public IActionResult DeleteSpecCart(int PID)
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

            // 找出該商品
            var existItem = cart.FirstOrDefault(c => c.ProductId == PID);
            cart.Remove(existItem); // 關鍵邏輯

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



        /*-------------------------------------------------------------------------------------------------------*/
        // 購物車初始Get 與 使用者 + - x, 重整購物車內容

        public CartViewModel BuildCartViewModel()
        {
            List<CartItem> cart;
            if (Request.Cookies.TryGetValue("LamboCart", out var cookieValue))
            {
                cart = JsonSerializer.Deserialize<List<CartItem>>(cookieValue) ?? new List<CartItem>();
            }
            else
            {
                cart = new List<CartItem>();
            }
            DBmanager db = new DBmanager();
            List<Product> products = new List<Product>();
            List<Product> res = new List<Product>();
            foreach (var item in cart)
            {
                products = db.getDetail(item.ProductId);
                res.Add(products[0]); // 取第一張img 當作該車款代表
            }
            CartViewModel model = new CartViewModel
            {
                CartItem = cart,
                Product = res
            };
            return model;
        }

        // 顯示購物清單 Cart/Index
        [HttpGet]
        public IActionResult Index()
        {
            var model = BuildCartViewModel();
            return View(model);
        }

        // 顯示購物清單 Cart/GetCartTable Ajax
        public IActionResult GetCartTable()
        {
            var model = BuildCartViewModel();
            return PartialView("_CartTable", model);
        }


        /*-------------------------------------------------------------------------------------------------------*/


        // 加入購物車 cookie邏輯
        [HttpPost]
        public IActionResult AddToCookie(string Series, string Model, int PID, int amount=1) // 選這兩變數原因, 配合DBmanager.cs 的 getDetail(string Series, string Model) // 多餘變數, 但先保留
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
                // 已存在,數量加 amount
                existItem.Quantity += amount;
            }
            else
            {
                // 不存在,新增商品
                cart.Add(new CartItem { ProductId = PID, Quantity = amount });
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

        // 每次換頁面 更新cart數量
        public IActionResult GetCartCount()
        {
            List<CartItem> cart;

            if (Request.Cookies.TryGetValue("LamboCart", out var cookieValue))
            {
                cart = JsonSerializer.Deserialize<List<CartItem>>(cookieValue) ?? new List<CartItem>();
            }
            else
            {
                cart = new List<CartItem>();
            }

            var totalQuantity = cart.Sum(c => c.Quantity);

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
