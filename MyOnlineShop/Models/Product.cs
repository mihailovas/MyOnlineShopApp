using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyOnlineShop.Models
{
    public class Product
    {

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public float Price { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public string ProductURL { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
