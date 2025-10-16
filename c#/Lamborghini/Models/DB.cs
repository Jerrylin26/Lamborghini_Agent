using Microsoft.AspNetCore.Mvc;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Data;
namespace Lamborghini.Models
{
    public class Member
    {
        public int MID { get; set; }

        [Display(Name = "帳號")]
        [Required(ErrorMessage = "必須輸入帳號")]
        [Remote(action: "CheckRepeatAccount", controller: "Account")] // 遠端驗證 確保無重複
        [RegularExpression(@"^.{5,20}$", ErrorMessage = "帳號至少5字,少於20字")]
        public string Account { get; set; }

        [Display(Name = "信箱")]
        [Required(ErrorMessage = "必須輸入Email")]
        [Remote(action: "CheckRepeatEmail", controller: "Account")] // 遠端驗證 確保無重複
        [MyEmail(ErrorMessage = "請輸入正確格式")] // 當作後端驗證 //送出後檢查
        [RegularExpression(@"^[\w\.-]+@[\w\.-]+\.\w{2,}$", ErrorMessage = "請輸入正確格式")] // 前端驗證 //萬能
        public string Email { get; set; }

        [Display(Name = "密碼")]
        [Required(ErrorMessage = "必須輸入密碼")]
        [RegularExpression(@"^.{5,20}$", ErrorMessage = "密碼至少5字,少於20字")]
        public string Password { get; set; }

        [Display(Name = "姓名")]
        [Required(ErrorMessage = "必須輸入姓名")]
        [RegularExpression(@"^.{1,20}$", ErrorMessage = "姓名不得超過20字")]
        public string Name { get; set; }

        [Display(Name = "地址")]
        [Required(ErrorMessage = "必須輸入地址")]
        public string Address { get; set; }

        [Display(Name = "電話")]
        [Required(ErrorMessage = "必須輸入電話")]
        [RegularExpression(@"^0\d{1,10}$", ErrorMessage = "請輸入正確格式")]
        [Remote(action: "CheckRepeatPhone", controller: "Account")] // 遠端驗證 確保無重複
        public string Phone { get; set; }

        [Display(Name = "性別")]
        [Required(ErrorMessage = "必須選取性別")]
        public string Gender { get; set; }

    }


    public class Product
    {
        // 產品資料表
        public int PID { get; set; }
        public string Img { get; set; }
        public double Price { get; set; }
        public int CarSeriesID { get; set; }
        public string CarSeries { get; set; } // CarSeriesID 對應的車系名稱
        public string CarModel { get; set; } // 車款名稱
        public bool IsDisplay { get; set; }
        public int ImageID { get; set; }

    }


    public class Order
    {
        // 產品訂購資料表
        public int OID { get; set; }
        public DateTime ODate { get; set; }
        public DateTime DDate { get; set; }
        public int PID { get; set; }
        public int MID { get; set; }
        public int Amount { get; set; }
        public float Price { get; set; }

    }

    public class AccessoryProduct
    {
        // 配件資料表
        public int APID { get; set; }
        public string Img { get; set; }
        public float Price { get; set; }
        public string Collection { get; set; } // CollectionID 對應的配件系列名稱
        public int CollectionID { get; set; }
        public string Item { get; set; } // 配件名稱
        public bool IsDisplay { get; set; }
        public string Description { get; set; } // 配件描述

    }

    public class AccessoryOrder
    {
        // 配件訂購資料表
        public int AOID { get; set; }
        public DateTime ODate { get; set; }
        public DateTime DDate { get; set; }
        public int APID { get; set; }
        public int MID { get; set; }
        public int Amount { get; set; }
        public float Price { get; set; }

    }

    // 用於儲存 cookie 的購物車項目
    public class CartItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    // 給 View 使用的 model
    public class CartViewModel
    {
        public List<CartItem> CartItem { get; set; }
        public List<Product> Product { get; set; }
    }

    public class CardInfo
    {
        [Required(ErrorMessage = "必須輸入卡號")]
        [RegularExpression(@"^[0-9]{4}-[0-9]{4}-[0-9]{4}-[0-9]{4}$", ErrorMessage = "輸入正確格式")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "必須輸入名稱")]
        public string Name { get; set; }

        [Required(ErrorMessage = "必須輸入cvc")]
        [RegularExpression(@"^[0-9]{3}$", ErrorMessage = "輸入正確格式")]
        public string Cvc { get; set; }

        [Required(ErrorMessage = "必須填入年")]
        public string Year { get; set;}

        [Required(ErrorMessage = "必須填入月")]
        public string Month { get; set; }
    }

    // 配合double 使用
    public class CheckoutViewModel
    {
        public double totalPrice { get; set; }
        public CardInfo Card { get; set; } = new CardInfo();
    }

}
