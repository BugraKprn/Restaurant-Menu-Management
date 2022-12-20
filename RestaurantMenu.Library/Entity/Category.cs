using OrionDAL.Web.Entities.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrionDAL.OAL;

namespace RestaurantMenu.Library.Entity
{
    public class Category : BaseEntity
    {

        public string CategoryName { get; set; } = "";
        public bool CategoryStatus { get; set; }
        public string CategoryIcon { get; set; } = "";
        public int CategoryOrder { get; set; }

    }
}
