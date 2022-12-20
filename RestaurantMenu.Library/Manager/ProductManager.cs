using OrionDAL.OAL;
using RestaurantMenu.Library.Entity;
using RestaurantMenu.Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantMenu.Library.Manager
{
    public class ProductManager : IProductService
    {
        public void Create(Product product)
        {
            product.Save();
        }

        public void Delete(int id)
        {
            Transaction.Instance.ExecuteNonQuery("Delete from Product where Id = @prm0", id);
        }

        public List<Product> GetbyCategory(int id)
        {
            return Transaction.Instance.ReadList<Product>("Select Product.ProductName, Product.ProductDescription, Product.ProductImage, Product.ProductPrice, Product.Category_Id as CategoryId from Product join Category on Product.Category_Id = Category.Id Where Category_Id = @prm0", id).ToList() ?? new List<Product>();
        }

        public Product GetById(int id)
        {
            return Transaction.Instance.Read<Product>("Select * from Product where Id=@prm0", id) ?? new Product();
        }

        public List<Product> GetList()
        {
            return Transaction.Instance.ReadList<Product>("Select Product.Id, Product.ProductName, Product.ProductDescription, Product.ProductImage, Product.ProductOrder, Product.ProductPrice, Product.Category_Id as CategoryId, Category.CategoryName as CategoryName from Product join Category on Product.Category_Id = Category.Id").ToList() ?? new List<Product>();
        }
    }
}