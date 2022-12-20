using Microsoft.AspNetCore.Mvc;
using RestaurantMenu.Library.Entity;
using RestaurantMenu.Library.Services;
using System;

namespace RestaurantQrMenu.ViewComponents
{
    public class Dashboard : ViewComponent
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        public Dashboard(
            IProductService productService,
            ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }
        public IViewComponentResult Invoke()
        {
            DashboardDto model = new DashboardDto()
            {
                ProductSetting = _productService.GetList(),
                CategorySetting = _categoryService.GetList()
            };
            return View(model);
        }
    }

    public class DashboardDto
    {
        public List<Product> ProductSetting { get; set; } = new List<Product>() { };
        public List<Category> CategorySetting { get; set; } = new List<Category>() { };
    }
}
