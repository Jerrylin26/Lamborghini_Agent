using Lamborghini.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Reflection;

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
                    .Select(f => Path.GetRelativePath(folderPath,f))
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
                Files = files ,
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
            return View("Details",model);
        }
    }
}
