using OrionDAL.OAL;
using RestaurantMenu.Library.Entity;
using RestaurantMenu.Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantMenu.Library.Manager
{
    public class CategoryManager : ICategoryService
    {
        public void Create(Category category)
        {
            category.Save();
        }

        public void Delete(int id)
        {
            Transaction.Instance.ExecuteNonQuery("Delete from Category where Id = @prm0", id);
        }

        public Category GetById(int id)
        {
            return Transaction.Instance.Read<Category>("Select * from Category Where Id = @prm0", id) ?? new Category();
        }

        public List<Category> GetByTrueCategory()
        {
            return Transaction.Instance.ReadList<Category>("Select * from Category Where CategoryStatus = @prm0", true).ToList() ?? new List<Category>();
        }

        public List<Category> GetList()
        {
            return Transaction.Instance.ReadList<Category>("Select * from Category").ToList() ?? new List<Category>();
        }

    }
}
