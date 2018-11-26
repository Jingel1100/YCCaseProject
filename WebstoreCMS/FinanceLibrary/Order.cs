using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using ProductsManagementSystemLibrary;

namespace FinanceLibrary
{
    public class Order
    {
        //Properties
        public int Id { get; set; }
        [Required(ErrorMessage = " {0} must contain a valid ordernumber.")]
        public string OrderNumber { get; set; }
        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public string OrderedProducts { get; set; }
        public string AmountsOrdered { get; set; }
        public bool Status { get; set; }
        [Required]
        public string TotalPrice { get; set; }
        public string StatusDescription { get; set; }
        public string OwnerId { get; set; }

        public void Create(Guid orderNumber, List<Product> product, string userId)
        {
            Order order = new Order()
            {
                Status = true
            };

            OwnerId = userId;
            OrderNumber = orderNumber.ToString();
            foreach (var item in product)
            {
                OrderedProducts += (item.Name.Trim() + ", ");
                AmountsOrdered += (item.Quantity.ToString().Trim() + ", ");
            }
            OrderedProducts.Trim(',');
            AmountsOrdered.Trim(',');
            StatusDescription = "This order is created";

        }


        public void Cancel()
        {
            Status = false;
            StatusDescription = "This order is canceled";
        }
    }
}

