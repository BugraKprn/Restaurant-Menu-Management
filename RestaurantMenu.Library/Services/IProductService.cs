using RestaurantMenu.Library.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantMenu.Library.Services
{
    public interface IProductService
    {
        List<Product> GetList();
        List<Product> GetbyCategory(int id);
        Product GetById(int id);
        void Create(Product product);
        void Delete(int id);
    }
}
