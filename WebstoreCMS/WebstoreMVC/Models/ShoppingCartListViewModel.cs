using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebstoreMVC.Models
{
    public class ShoppingCartListViewModel : ListViewModel<ProductListItemViewModel>
    {
        public int Id { get; set; }
        public List<ProductListItemViewModel> Products { get; set; }
        public string Total { get; set; }

        public ShoppingCartListViewModel() : base()
        {

        }
    }
}
