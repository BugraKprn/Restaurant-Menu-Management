using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OrionDAL.OAL;
using OrionDAL.Web.Entities.Core;
using RestaurantMenu.Library.Entity;
using RestaurantMenu.Library.Helpers;
using RestaurantMenu.Library.Services;


namespace RestaurantQrMenu.Controllers
{
    public class trController : Controller
    {
        private readonly ILogger<trController> _logger;
        private IProductService _productService;
        private ICategoryService _categoryService;
        public trController(
            ILogger<trController> logger
            , IProductService productService
            , ICategoryService categoryService)
        {
            _logger = logger;
            _productService = productService;
            _categoryService = categoryService;
        }
        public IActionResult Index()
        {
            ViewData["Title"] = "Menu";
            MenuDto model = new MenuDto()
            {
                ProductModel = _productService.GetList(),
                CategoryModel = _categoryService.GetByTrueCategory()
            };
            return View(model);
        }

        public class MenuDto
        {
            public List<Product> ProductModel { get; set; } = new List<Product>() { };
            public List<Category> CategoryModel { get; set; } = new List<Category>() { };
        }
    }
}
