using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyOnlineShop.Models.ViewModel
{
    public class CategoryVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CategoryURL { get; set; }

        public CategoryVM()
        {

        }

        public CategoryVM(Category row)
        {
            Id = row.Id;
            Name = row.Name;
            CategoryURL = row.CategoryURL;
        }
    }
}