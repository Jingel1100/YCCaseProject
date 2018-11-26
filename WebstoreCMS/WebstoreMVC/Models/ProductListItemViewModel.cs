using ProductsManagementSystemLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebstoreMVC.Models
{
    public class ProductListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual File Image { get; set; }
        public string Price { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        public string Quantity { get; set; }
        public string Subtotal { get; set; }
    }
}