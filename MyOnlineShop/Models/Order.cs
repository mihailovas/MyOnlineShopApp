using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyOnlineShop.Models
{
    public class Order
    {
        
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        





    }
}