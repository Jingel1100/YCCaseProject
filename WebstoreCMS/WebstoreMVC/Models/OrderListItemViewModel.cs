using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebstoreMVC.Models
{
    public class OrderListItemViewModel
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string TotalPrice { get; set; }
        public string OrderedProducts { get; set; }
        public bool Status { get; set; }
        public string OwnerId { get; set; }
        public string StatusDescription { get; set; }
    }
}
