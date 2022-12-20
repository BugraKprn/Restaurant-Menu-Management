using OrionDAL.Web.Entities.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrionDAL.ActiveRecord;
using OrionDAL.OAL.Metadata;
using OrionDAL.OAL;

namespace RestaurantMenu.Library.Entity
{
    public class Product : BaseEntity
    {
        public SharpPointer<Category> Category { get; set; }
        public int ProductOrder { get; set; }
        public string ProductName { get; set; } = "";
        public string ProductPrice { get; set; } = "";
        public string ProductDescription { get; set; } = "";
        public string ProductImage { get; set; } = "";
        [NonPersistent]
        public int CategoryId { get; set; }
        [NonPersistent]
        public string CategoryName { get; set; } = "";
        public Product()
        {
            Category = new SharpPointer<Category>();
        }
    }
}
