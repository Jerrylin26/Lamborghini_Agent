using Lamborghini.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;


namespace Lamborghini.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Checkout()
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
            double totalPrice = 0;
            foreach (var item in cart)
            {
                var product = db.getDetail(item.ProductId)[0]; // 只取第一個產品,由於都是同個商品
                totalPrice += product.Price * item.Quantity;

            }

            return View(totalPrice);

        }
        public IActionResult Confirm()
        {
            return View();
        }
    }
}
