using Lamborghini.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace Lamborghini.Controllers
{
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _env;
        public ProductController(IWebHostEnvironment env)
        {
            _env = env;
        }



        /*---------------------------------------------------------------------------------*/
        // 產品總覽頁面

        // 首次進入頁面
        [HttpGet]
        public IActionResult List()
        {
            string folderPath;

            // 用session記錄上次選擇的車款 讓返回頁面正常
            var default_car_type = HttpContext.Session.GetString("LastCarType") ?? "Temerario";
            if (default_car_type == "Revuelto" || default_car_type == "Temerario" || default_car_type == "Urus_SE")
            {
                folderPath = Path.Combine(_env.WebRootPath, "img", "Lamborghini_pic", default_car_type);
            }
            else
            {
                folderPath = Path.Combine(_env.WebRootPath, "img", default_car_type);
            }

            var allFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories).ToList();
            List<string?> randomFiles = new List<string?>();

            if (allFiles.Any())
            {
                var rd = new Random();

                randomFiles = allFiles
                    .OrderBy(x => rd.Next())
                    .Take(9)
                    .Select(f => Path.GetRelativePath(folderPath, f))
                    .ToList();
            }

            var model = new detailViewimgModel
            {
                Files = randomFiles,
                Title = default_car_type

            };

            return View(model);
        }

        // Ajax更新圖片
        [HttpPost]
        public IActionResult List(string car_type)
        {
            // 用session記錄上次選擇的車款 讓返回頁面正常
            HttpContext.Session.SetString("LastCarType", car_type);

            var folderPath = Path.Combine(_env.WebRootPath, "img", "Lamborghini_pic", car_type);
            var allFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories).ToList();
            List<string?> randomFiles = new List<string?>();

            if (allFiles.Any())
            {
                var rd = new Random();

                randomFiles = allFiles
                    .OrderBy(x => rd.Next())
                    .Take(9)
                    .Select(f => Path.GetRelativePath(folderPath, f))
                    .ToList();
            }

            // Ajax
            return Json(new { title = car_type, files = randomFiles });

        }

        // Ajax更新圖片 另一類型資料夾結構 (不同類別)
        [HttpPost]
        public IActionResult List_1(string car_type)
        {
            // 用session記錄上次選擇的車款 讓返回頁面正常
            HttpContext.Session.SetString("LastCarType", car_type);



            var folderPath = Path.Combine(_env.WebRootPath, "img", car_type);
            var allFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories).ToList();
            List<string?> randomFiles = new List<string?>();

            if (allFiles.Any())
            {
                var rd = new Random();

                randomFiles = allFiles
                    .OrderBy(x => rd.Next())
                    .Take(9)
                    .Select(f => Path.GetRelativePath(folderPath, f))
                    .ToList();
            }

            // Ajax
            return Json(new { title = car_type, files = randomFiles });

        }




        /*---------------------------------------------------------------------------------*/
        // 進入單一車款頁面

        //[HttpPost]
        public IActionResult Details(string car_type, string car_color)
        {
            string default_car_type = "Urus_SE";
            string default_car_color = "Grigio China Shiny";


            //首次點選預設
            if (string.IsNullOrEmpty(car_type) || string.IsNullOrEmpty(car_color))
            {
                car_type = default_car_type;
                car_color = default_car_color;
            }


            var folderPath = Path.Combine(_env.WebRootPath, "img", "Lamborghini_pic", car_type, car_color);

            //防呆
            if (!Directory.Exists(folderPath))
            {
                car_type = default_car_type;
                car_color = default_car_color;
                folderPath = Path.Combine(_env.WebRootPath, "img", "Lamborghini_pic", car_type, car_color);
            }

            List<string> files = Directory.Exists(folderPath) ? Directory.GetFiles(folderPath).Select(Path.GetFileName).ToList() : new List<string>();

            var model = new detailViewimgModel
            {
                Title = car_type,
                Color = car_color,
                Files = files,
                Count = files.Count
            };
            return View(model);
        }

        //  另一類型資料夾結構 (不同類別)
        //[HttpPost]
        public IActionResult Details_1(string car_type, string subtype)
        {
            string default_car_type = "ConceptCars_Lambo_pic";
            string default_car_subtype = "Estoque";


            //首次點選預設
            if (string.IsNullOrEmpty(car_type) || string.IsNullOrEmpty(subtype))
            {
                car_type = default_car_type;
                subtype = default_car_subtype;
            }


            var folderPath = Path.Combine(_env.WebRootPath, "img", car_type, subtype);

            //防呆
            if (!Directory.Exists(folderPath))
            {
                car_type = default_car_type;
                subtype = default_car_subtype;
                folderPath = Path.Combine(_env.WebRootPath, "img", car_type, subtype);
            }

            List<string> files = Directory.Exists(folderPath) ? Directory.GetFiles(folderPath).Select(Path.GetFileName).ToList() : new List<string>();

            var model = new detailViewimgModel
            {
                Title = car_type,
                Subtype = subtype,
                Files = files,
                Count = files.Count
            };
            return View("Details", model);
        }



        /*---------------------------------------------------------------------------------*/
        // 產品初始匯入資料庫 Product ProductImage 資料表


        //由於json名稱與資料庫欄位不符 需建立一個class來對應
        public class PriceItem
        {
            public string title { get; set; }
            public float price { get; set; }
        }

        

        public IActionResult addProduct(Product user)
        {

            string fileName;
            string filePath;
            List<Product> products = new List<Product>();
            List<Product> productforcolor = new List<Product>();
            List<PriceItem> productPrice = new List<PriceItem>();


            DBmanager dbmanager = new DBmanager();
            try
            {

                // 讀取 價格 .json
                fileName = "Lambo_price.json";
                filePath = Path.Combine(_env.WebRootPath, "file", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    string jsonString = System.IO.File.ReadAllText(filePath);
                    productPrice = JsonSerializer.Deserialize<List<PriceItem>>(jsonString) ?? new List<PriceItem>(); // Price
                }
                else
                {

                    Debug.WriteLine($"File not found: {filePath}");
                }

                /**-------------------------------------------------------------------------------**/
                // 讀取 Aventador_Lambo_pic
                fileName = "Aventador_Lambo_pic";
                filePath = Path.Combine(_env.WebRootPath, "img", fileName);

                if (System.IO.Directory.Exists(filePath))
                {
                    string[] carSeriesPath = Directory.GetDirectories(filePath); // 完整路徑
                    foreach (var carPath in carSeriesPath)
                    {
                        string[] imgs = Directory.GetFiles(carPath);
                        string car = Path.GetFileName(carPath); // 只要資料夾名稱
                        foreach (var img in imgs)
                        {
                            string relativePath = Path.GetRelativePath(_env.WebRootPath, img)
                                    .Replace("\\", "/");
                            products.Add(new Product { CarSeries = "Aventador", CarModel = car, Img = relativePath }); // CarSeries CarModel Img
                        }
                    }

                }
                else
                {

                    Debug.WriteLine($"File not found: {filePath}");
                }
                /**-------------------------------------------------------------------------------**/
                // 讀取 ConceptCars_Lambo_pic
                fileName = "ConceptCars_Lambo_pic";
                filePath = Path.Combine(_env.WebRootPath, "img", fileName);

                if (System.IO.Directory.Exists(filePath))
                {
                    string[] carSeriesPath = Directory.GetDirectories(filePath); // 完整路徑
                    foreach (var carPath in carSeriesPath)
                    {
                        string[] imgs = Directory.GetFiles(carPath);
                        string car = Path.GetFileName(carPath); // 只要資料夾名稱
                        foreach (var img in imgs)
                        {
                            string relativePath = Path.GetRelativePath(_env.WebRootPath, img)
                                    .Replace("\\", "/");
                            products.Add(new Product { CarSeries = "ConceptCars", CarModel = car, Img = relativePath }); // CarSeries CarModel Img
                        }
                    }
                }
                else
                {

                    Debug.WriteLine($"File not found: {filePath}");
                }
                /**-------------------------------------------------------------------------------**/
                // 讀取 Huracán_Lambo_pic
                fileName = "Huracán_Lambo_pic";
                filePath = Path.Combine(_env.WebRootPath, "img", fileName);

                if (System.IO.Directory.Exists(filePath))
                {
                    string[] carSeriesPath = Directory.GetDirectories(filePath); // 完整路徑
                    foreach (var carPath in carSeriesPath)
                    {
                        string[] imgs = Directory.GetFiles(carPath);
                        string car = Path.GetFileName(carPath); // 只要資料夾名稱
                        foreach (var img in imgs)
                        {
                            string relativePath = Path.GetRelativePath(_env.WebRootPath, img)
                                    .Replace("\\", "/");
                            products.Add(new Product { CarSeries = "Huracán", CarModel = car, Img = relativePath }); // CarSeries CarModel Img
                        }
                    }
                }
                else
                {

                    Debug.WriteLine($"File not found: {filePath}");
                }
                /**-------------------------------------------------------------------------------**/
                // 讀取 LimitedModel_Lambo_pic
                fileName = "LimitedModel_Lambo_pic";
                filePath = Path.Combine(_env.WebRootPath, "img", fileName);

                if (System.IO.Directory.Exists(filePath))
                {
                    string[] carSeriesPath = Directory.GetDirectories(filePath); // 完整路徑
                    foreach (var carPath in carSeriesPath)
                    {
                        string[] imgs = Directory.GetFiles(carPath);
                        string car = Path.GetFileName(carPath); // 只要資料夾名稱
                        foreach (var img in imgs)
                        {
                            string relativePath = Path.GetRelativePath(_env.WebRootPath, img)
                                    .Replace("\\", "/");
                            products.Add(new Product { CarSeries = "LimitedModel", CarModel = car, Img = relativePath }); // CarSeries CarModel Img
                        }
                    }
                }
                else
                {

                    Debug.WriteLine($"File not found: {filePath}");
                }
                /**-------------------------------------------------------------------------------**/
                // 讀取 sportCar_Lambo_pic
                fileName = "sportCar_Lambo_pic";
                filePath = Path.Combine(_env.WebRootPath, "img", fileName);

                if (System.IO.Directory.Exists(filePath))
                {
                    string[] carSeriesPath = Directory.GetDirectories(filePath); // 完整路徑
                    foreach (var carPath in carSeriesPath)
                    {
                        string[] imgs = Directory.GetFiles(carPath);
                        string car = Path.GetFileName(carPath); // 只要資料夾名稱
                        foreach (var img in imgs)
                        {
                            string relativePath = Path.GetRelativePath(_env.WebRootPath, img)
                                    .Replace("\\", "/");
                            products.Add(new Product { CarSeries = "sportCar", CarModel = car, Img = relativePath }); // CarSeries CarModel Img
                        }
                    }
                }
                else
                {

                    Debug.WriteLine($"File not found: {filePath}");
                }
                /**-------------------------------------------------------------------------------**/
                // 讀取 Revuelto
                fileName = "Revuelto";
                filePath = Path.Combine(_env.WebRootPath, "img", "Lamborghini_pic", fileName);

                if (System.IO.Directory.Exists(filePath))
                {
                    string[] carSeriesPath = Directory.GetDirectories(filePath); // 完整路徑
                    foreach (var carPath in carSeriesPath)
                    {
                        string[] imgs = Directory.GetFiles(carPath);
                        string car = Path.GetFileName(carPath); // 只要資料夾名稱
                        foreach (var img in imgs)
                        {
                            string relativePath = Path.GetRelativePath(_env.WebRootPath, img)
                                    .Replace("\\", "/");
                            productforcolor.Add(new Product { CarSeries = "Revuelto", CarModel = car, Img = relativePath }); // CarSeries CarModel Img
                        }
                    }
                }
                else
                {

                    Debug.WriteLine($"File not found: {filePath}");
                }
                /**-------------------------------------------------------------------------------**/
                // 讀取 Temerario
                fileName = "Temerario";
                filePath = Path.Combine(_env.WebRootPath, "img", "Lamborghini_pic", fileName);

                if (System.IO.Directory.Exists(filePath))
                {
                    string[] carSeriesPath = Directory.GetDirectories(filePath); // 完整路徑
                    foreach (var carPath in carSeriesPath)
                    {
                        string[] imgs = Directory.GetFiles(carPath);
                        string car = Path.GetFileName(carPath); // 只要資料夾名稱
                        foreach (var img in imgs)
                        {
                            string relativePath = Path.GetRelativePath(_env.WebRootPath, img)
                                    .Replace("\\", "/");
                            productforcolor.Add(new Product { CarSeries = "Temerario", CarModel = car, Img = relativePath }); // CarSeries CarModel Img
                        }
                    }
                }
                else
                {

                    Debug.WriteLine($"File not found: {filePath}");
                }
                /**-------------------------------------------------------------------------------**/
                // 讀取 Urus_SE
                fileName = "Urus_SE";
                filePath = Path.Combine(_env.WebRootPath, "img", "Lamborghini_pic", fileName);

                if (System.IO.Directory.Exists(filePath))
                {
                    string[] carSeriesPath = Directory.GetDirectories(filePath); // 完整路徑
                    foreach (var carPath in carSeriesPath)
                    {
                        string[] imgs = Directory.GetFiles(carPath);
                        string car = Path.GetFileName(carPath); // 只要資料夾名稱

                        foreach (var img in imgs)
                        {
                            string relativePath = Path.GetRelativePath(_env.WebRootPath, img)
                                    .Replace("\\", "/");
                            productforcolor.Add(new Product { CarSeries = "Urus_SE", CarModel = car, Img = relativePath }); // CarSeries CarModel Img
                        }
                    }

                }
                else
                {

                    Debug.WriteLine($"File not found: {filePath}");
                }
                /**-------------------------------------------------------------------------------**/

                // 寫入資料庫
                var productdb = (from pp in productPrice
                                 join p in products on pp.title equals p.CarModel
                                 select new Product
                                 {
                                     CarModel = p.CarModel,
                                     Price = pp.price,
                                     CarSeries = p.CarSeries,
                                     Img = p.Img,
                                     IsDisplay = true
                                 }).ToList();

                // 寫入資料庫 給顏色分類的車
                var productdb2 = (from pp in productPrice
                                  join p in productforcolor on pp.title equals p.CarSeries
                                  select new Product
                                  {
                                      CarModel = p.CarModel,
                                      Price = pp.price,
                                      CarSeries = p.CarSeries,
                                      Img = p.Img,
                                      IsDisplay = true
                                  }).ToList();


                Debug.WriteLine("要寫入資料數量：" + productdb.Count);
                Debug.WriteLine("要寫入資料數量：" + productdb2.Count);


                // 有兩個因為資料表結構不同
                // 存 DB 並取得 PID 再存圖片
                foreach (var productGroup in productdb.GroupBy(p => new { p.CarSeries, p.CarModel }))
                {
                    var firstProduct = productGroup.First();

                    // 先寫入 Product，取得 PID
                    int newProductId = dbmanager.newProduct(firstProduct);

                    // 再把所有圖片寫入 ProductImage
                    foreach (var imgProduct in productGroup)
                    {
                        dbmanager.newProductImage(newProductId, imgProduct.Img);
                    }
                }
                Debug.WriteLine("寫入資料庫完成");

                // 分別寫入資料庫 .newProduct .newProductImage 不同資料表
                // 存 DB 並取得 PID 再存圖片
                foreach (var productGroup in productdb2.GroupBy(p => new { p.CarSeries, p.CarModel }))
                {
                    var firstProduct = productGroup.First();

                    // 先寫入 Product，取得 PID
                    int newProductId = dbmanager.newProduct(firstProduct);

                    // 再把所有圖片寫入 ProductImage
                    foreach (var imgProduct in productGroup)
                    {
                        dbmanager.newProductImage(newProductId, imgProduct.Img);
                    }
                }
                Debug.WriteLine("寫入資料庫完成");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return View();
        }

        // 產品初始匯入資料庫 AccessoryProduct 資料表
        public class PriceItem_Balenciaga
        {
            public string title { get; set; }
            public string detail { get; set; } // 專門給 Balenciaga_Lambo_28.json 用的
            public string img_Balenciaga { get; set; } // 專門給 Balenciaga_Lambo_28.json 用的
            public string price { get; set; } // 專門給 Balenciaga_Lambo_28.json 用的
        }
        public IActionResult addAccessoryProduct(AccessoryProduct user)
        {
            string fileName;
            string filePath;
            List<PriceItem_Balenciaga> products = new List<PriceItem_Balenciaga>(); // 存其餘.json資料用
            List<PriceItem_Balenciaga> imgObj = new List<PriceItem_Balenciaga>(); // 只存圖片用



            DBmanager dbmanager = new DBmanager();
            try
            {
                // 讀取img
                filePath = Path.Combine(_env.WebRootPath, "img", "Balenciaga_Lambo_pic");

                if (System.IO.Directory.Exists(filePath))
                {
                    string[] BalenSeriesPath = Directory.GetDirectories(filePath); // 完整路徑
                    foreach (var BalenPath in BalenSeriesPath)
                    {
                        string[] imgs = Directory.GetFiles(BalenPath);
                        string name = Path.GetFileName(BalenPath); // 只要資料夾名稱

                        foreach (var img in imgs)
                        {
                            string relativePath = Path.GetRelativePath(_env.WebRootPath, img)
                                    .Replace("\\", "/");
                            imgObj.Add(new PriceItem_Balenciaga { title = name, img_Balenciaga = relativePath }); // Item Img
                        }
                    }

                }
                else
                {

                    Debug.WriteLine($"File not found: {filePath}");
                }
                /**-------------------------------------------------------------------------------**/
                // 讀取  Balenciaga_Lambo_28.json
                fileName = "Balenciaga_Lambo_28.json";
                filePath = Path.Combine(_env.WebRootPath, "file", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    string jsonString = System.IO.File.ReadAllText(filePath);
                    products = JsonSerializer.Deserialize<List<PriceItem_Balenciaga>>(jsonString) ?? new List<PriceItem_Balenciaga>(); // Price
                    var productdb = (from p in products
                                     join pp in imgObj on p.title equals pp.title
                                     select new AccessoryProduct
                                      {
                                          Collection = "Balenciaga_Lambo",
                                          Price = float.Parse(p.price),
                                          Item = p.title,
                                          Img = pp.img_Balenciaga, // 多張pic
                                          Description = p.detail,
                                          IsDisplay = true,
                                      }).ToList();


                    Debug.WriteLine("要寫入資料數量：" + productdb.Count);

                    // 分別寫入資料庫 .newAccessoryProduct .newAccessoryProduct 不同資料表
                    // 存 DB 並取得 APID 再存圖片
                    foreach (var productGroup in productdb.GroupBy(p => new { p.Collection, p.Item }))
                    {
                        var firstProduct = productGroup.First();

                        // 先寫入 Product，取得 PID
                        int newProductId = dbmanager.newAccessoryProduct(firstProduct);

                        // 再把所有圖片寫入 ProductImage
                        foreach (var imgProduct in productGroup)
                        {
                            dbmanager.newAccessoryProductImage(newProductId, imgProduct.Img);
                        }
                    }
                    Debug.WriteLine("寫入資料庫完成");
                }
                else
                {

                    Debug.WriteLine($"File not found: {filePath}");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            return Content("測試成功");
        }
    }
}

            

        
