using RestaurantMenu.Library.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantMenu.Library.Services
{
    public interface ICategoryService
    {
        List<Category> GetList();
        List<Category> GetByTrueCategory();
        Category GetById(int id);
        void Create(Category category);
        void Delete(int id);
    }
}
