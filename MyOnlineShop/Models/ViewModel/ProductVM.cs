using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyOnlineShop.Models.ViewModel
{
    public class ProductVM
    {

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public float Price { get; set; }
        public string Description { get; set; }
        public string ProductURL { get; set; }
        public string CategoryName { get; set; }
        [Required]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<string> GalleryImages { get; set; }

        public ProductVM()
        {

        }
        public ProductVM(Product row)
        {
            Id = row.Id;
            Name = row.Name;
            Price = row.Price;
            Description = row.Description;
            ProductURL = row.ProductURL;
            CategoryName = row.CategoryName;
            CategoryId = row.CategoryId;

        }


    }
}