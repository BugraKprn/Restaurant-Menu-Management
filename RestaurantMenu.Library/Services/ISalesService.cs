using RestaurantMenu.Library.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantMenu.Library.Services
{
    public interface ISalesService
    {
        List<Sales> GetList();
        Sales GetById(int id);
        void Create(Sales sales);
        void Delete(int id);
    }
}
