using Microsoft.AspNetCore.Mvc;
using System.Collections.Specialized;
using System.Data;
namespace Lamborghini.Models
{
    public class Member
    {
        // 會員資料表
        public int MID { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class Product
    {
        // 產品資料表
        public int PID { get; set; }
        public string Img { get; set; }
        public float Price { get; set; }
        public string CarSeries { get; set; }
        public string CarModel { get; set; }
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
        public string Collection { get; set; }
        public string Item { get; set; }
        public bool IsDisplay { get; set; }
        public string Description { get; set; }

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

}
