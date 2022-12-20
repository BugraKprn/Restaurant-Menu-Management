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
    public class SalesManager : ISalesService
    {
        public void Create(Sales sales)
        {
            sales.Save();
        }

        public void Delete(int id)
        {
            Transaction.Instance.ExecuteNonQuery("Delete from Sales where Id = @prm0", id);
        }

        public Sales GetById(int id)
        {
            return Transaction.Instance.Read<Sales>("Select * from Sales where Id=@prm0", id) ?? new Sales();
        }

        public List<Sales> GetList()
        {
            return Transaction.Instance.ReadList<Sales>("Select Sales.Id, Sales.Quantity, Sales.Date, Sales.TotalAmount, Product.ProductName as ProductName, Product.ProductPrice as ProductPrice, Product.Id as ProductId from Sales join Product on Sales.Product_Id = Product.Id").ToList() ?? new List<Sales>();
        }
    }
}