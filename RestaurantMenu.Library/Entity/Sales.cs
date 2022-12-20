using OrionDAL.ActiveRecord;
using OrionDAL.OAL;
using OrionDAL.Web.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantMenu.Library.Entity
{
    public class Sales : BaseEntity
    {
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public string TotalAmount { get; set; }
        public SharpPointer<Product> Product { get; set; }

        [NonPersistent]
        public string ProductId { get; set; } = "";

        [NonPersistent]
        public string ProductName { get; set; } = "";

        [NonPersistent]
        public string ProductPrice { get; set; } = "";
        public Sales()
        {
            Product = new SharpPointer<Product>();
        }
    }
}
