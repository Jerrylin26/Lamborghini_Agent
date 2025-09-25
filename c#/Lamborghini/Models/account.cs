using Microsoft.AspNetCore.Mvc;
using System.Data;
namespace Lamborghini.Models
{
    public class account
    {
        // 與資料表名稱相同
        public int ID { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public double age { get; set; }
    }
}
