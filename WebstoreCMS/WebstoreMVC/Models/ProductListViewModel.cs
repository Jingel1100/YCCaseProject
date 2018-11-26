using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebstoreMVC.Models
{
    public class ProductListViewModel : ListViewModel<ProductListItemViewModel>
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }

        public ProductListViewModel() : base() { }
    }
}