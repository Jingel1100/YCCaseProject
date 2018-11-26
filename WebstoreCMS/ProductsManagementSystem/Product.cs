using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductsManagementSystemLibrary
{
    [Serializable]
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string Price { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public virtual File Image { get; set; }

        public int Stock { get; set; }
        public int Quantity { get; set; }
        public string Subtotal { get; set; }

        public bool InStock(int number)
        {
            return number <= this.Stock ? true : false;
        }

        public void GetSubtotal()
        {
            if (Price.IndexOfAny(new char[] { ',', '.' }) == -1)
            {
                Price = Price + ".00";
            }
            decimal.TryParse(Price, out decimal productPrice);
            Subtotal = ((Quantity * productPrice) / 100).ToString();
        }
    }
}
