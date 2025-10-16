using Lamborghini.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Lamborghini.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }


        public IActionResult Register()
        {
            return View();
        }

        // 登出
        [HttpGet]
        public IActionResult Logout()
        {
            Request.Cookies.TryGetValue("LoginAccount", out string? loginAccount);
            if (loginAccount != null)
            {
                Response.Cookies.Delete("LoginAccount");
                return RedirectToAction("Index", "Home");
            }
            return NoContent();


        }

        // 檢查是否已登入
        [HttpGet]
        public IActionResult LoginCheck()
        {
            Request.Cookies.TryGetValue("LoginAccount", out string? loginAccount);
            if (loginAccount != null)
            {
                return Json(new { isLoggedIn = true, message = "已登入", loginName = loginAccount });
            }
            else
            {
                return Json(new { isLoggedIn = false, message = "未登入" });
            }
        }

        // 送出登入表單
        [HttpPost]
        public IActionResult Login_post(string Account, string Password)
        {
            Debug.WriteLine("Account: " + Account);
            Debug.WriteLine("Password: " + Password);
            // 驗證通過 → 登入
            DBmanager db = new DBmanager();

            List<Member> accounts = db.getAccount();
            if (accounts == null)
            {
                throw new Exception("db.getAccount() 回傳 null，請檢查 DBmanager.getAccount() 是否有 return new List<Member>()。");
            }


            foreach (var account in accounts)
            {
                if (account.Account == Account && account.Password == Password)
                {
                    HttpContext.Response.Cookies.Append(
                        "LoginAccount",
                        Account,
                        new CookieOptions
                        {
                            Expires = DateTimeOffset.Now.AddDays(0.04),
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict
                        }
                    );

                    return Json(new { success = true, message = "登入成功", redirectUrl = Url.Action("Index", "Home") });
                }
            }
            return Json(new { success = false, message = "帳號或密碼有誤" });
        }


        // 送出註冊表單
        [HttpPost]
        public IActionResult Register_post(Member member)
        {
            if (!ModelState.IsValid)
            {
                // 有錯誤 → 重新回傳 View，紅字會顯示
                return View("Register", member);
            }

            // 驗證通過 → 做註冊或儲存資料庫
            DBmanager db = new DBmanager();
            db.newAccount(member);
            TempData["Message"] = "註冊成功";
            return RedirectToAction("Login");
        }

        public IActionResult CheckRepeatAccount(string Account)
        {
            DBmanager db = new DBmanager();

            List<Member> accounts = db.getAccount();
            foreach (var account in accounts)
            {
                if (account.Account == Account)
                {
                    return Json($"{Account} 被註冊過了");
                }
            }
            return Json(true);
        }

        public IActionResult CheckRepeatEmail(string Email)
        {
            DBmanager db = new DBmanager();

            List<Member> accounts = db.getAccount();
            foreach (var account in accounts)
            {
                if (account.Email == Email)
                {
                    return Json($"{Email} 被註冊過了");
                }
            }
            return Json(true);
        }

        public IActionResult CheckRepeatPhone(string Phone)
        {
            DBmanager db = new DBmanager();

            List<Member> accounts = db.getAccount();
            foreach (var account in accounts)
            {
                if (account.Phone == Phone)
                {
                    return Json($"{Phone} 被註冊過了");
                }
            }
            return Json(true);
        }

    }
}
