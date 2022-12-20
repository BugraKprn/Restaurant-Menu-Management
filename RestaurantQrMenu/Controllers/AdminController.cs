using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OrionDAL.OAL;
using OrionDAL.Web.Entities.Core;
using RestaurantMenu.Library.Helpers;
using RestaurantMenu.Library.Services;

namespace RestaurantQrMenu.Controllers
{
    public class AdminController : Controller
    {
        private IProductService _productService;
        private ICategoryService _categoryService;
        private ISalesService _salesService;

        public AdminController(
            ICategoryService categoryService,
            IProductService productService,
            ISalesService salesService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _salesService = salesService;
        }

        [Route("admin")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Ana Sayfa";
            return View();
        }

        public IActionResult CategoryPage()
        {
            ViewData["Title"] = "Kategoriler";
            return View();
        }

        [Route("getcategory")]
        [HttpGet]
        public async Task<IActionResult> GetCategory()
        {
            ApiResult result = new ApiResult();
            try
            {
                result.Data = _categoryService.GetList();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            //Write your Insert code here;
            return await Task.FromResult(Ok(result));
        }

        public IActionResult ProductPage()
        {
            ViewData["Title"] = "Ürünler";
            return View();
        }

        [Route("getproduct")]
        [HttpGet]
        public async Task<IActionResult> GetProduct()
        {
            ApiResult result = new ApiResult();
            try
            {
                result.Data = _productService.GetList();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            //Write your Insert code here;
            return await Task.FromResult(Ok(result));
        }

        public IActionResult SalesPage()
        {
            ViewData["Title"] = "Satış Ve Hareketler";
            return View();
        }

        [Route("getsales")]
        [HttpGet]
        public async Task<IActionResult> GetSales()
        {
            ApiResult result = new ApiResult();
            try
            {
                result.Data = _salesService.GetList();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            //Write your Insert code here;
            return await Task.FromResult(Ok(result));
        }

        [HttpPost]
        public string UploadFile(IFormFile myFile)
        {

            var targetLocation = Environment.CurrentDirectory + "/StaticFiles";
            string dosyaUzantisi = Path.GetExtension(myFile.FileName).ToLower();
            Guid g = Guid.NewGuid();


            try
            {
                using (var fileStream = System.IO.File.Create(targetLocation + "/" + g.ToString() + dosyaUzantisi))
                {
                    myFile.CopyTo(fileStream);
                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return g.ToString() + dosyaUzantisi;
        }
        public JsonResult OrionSave(IFormCollection form)
        {
            var values = form["values"];
            var table = form["tablo"];
            var keyValue = Convert.ToInt32(form["key"]);

            var typeTable = OrionDAL.OAL.Metadata.DataDictionary.Instance.GetTypeofEntity(table);
            var obj = (BaseEntity)(keyValue > 0 ? Transaction.Instance.Read(typeTable, keyValue) : Activator.CreateInstance(typeTable));

            JsonConvert.PopulateObject(values, obj);

            obj.Save();

            return Json(new { data = obj });
        }
        public JsonResult OrionDelete(IFormCollection form)
        {
            var table = form["tablo"];
            var keyValue = Convert.ToInt32(form["key"]);

            var typeTable = OrionDAL.OAL.Metadata.DataDictionary.Instance.GetTypeofEntity(table);
            var obj = (BaseEntity)Transaction.Instance.Read(typeTable, keyValue);
            //if (table == "BagisTanim")
            //{
            //    Transaction.Instance.ExecuteNonQuery("delete from BagisSepeti where BagisTanim_Id=@prm0", obj.Id);
            //}
            //else if (table == "Ogrenci")
            //{
            //    Transaction.Instance.ExecuteNonQuery("delete from BagisSepeti where OgrenciId=@prm0", obj.Id);
            //    Transaction.Instance.ExecuteNonQuery("delete from BasariliOdemeLog where OgrenciId=@prm0", obj.Id);
            //    Transaction.Instance.ExecuteNonQuery("delete from BasarisizOdemeLog where OgrenciId=@prm0", obj.Id);
            //    Transaction.Instance.ExecuteNonQuery("delete from OdemeAraTablosu where OgrenciId=@prm0", obj.Id);
            //    Transaction.Instance.ExecuteNonQuery("delete from OgrenciBurs where Ogrenci_Id=@prm0", obj.Id);
            //}
            obj.Delete();

            return Json(new { data = "{}" });
        }
    }
}
