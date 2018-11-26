using DeliverySystemLibrary;
using FinanceLibrary;
using ProductsManagementSystemLibrary;
using System;
using System.Collections.Generic;

namespace ShoppingCartLibrary
{
    [Serializable]
    public class ShoppingCart
    {
        public int Id { get; set; }
        public List<Product> Products { get; set; }
        public string Total { get; set; }

        public ShoppingCart()
        {
            Products = new List<Product>();
            Total = "0,00";
        }

        public void Add(Product product)
        {
            var match = this.Products.Find(p => p.Id == product.Id);

            if (match == null)
            {
                this.Products.Add(product);
            }
            int index = this.Products.FindIndex(p => p.Id == product.Id);
            this.Products[index].Quantity += 1;
            this.Products[index].GetSubtotal();
        }

        public void Remove(Product product)
        {
            int index = this.Products.FindIndex(p => p.Id == product.Id);
            if (this.Products[index].Quantity >= 2)
            {
                this.Products[index].Quantity -= 1;
                this.Products[index].GetSubtotal();
            }
            else
            {
                this.Products.RemoveAt(index);
            }
        }

        public void CalculateTotal()
        {
            decimal total = 0;
            foreach (var item in Products)
            {
                decimal.TryParse(item.Subtotal, out decimal price);
                total += price;
            }

            Total = total.ToString();
        }

        public Order ConfirmOrder(string userId)
        {
            foreach (var item in Products)
            {
                if (item.InStock(item.Quantity))
                {
                    item.Stock -= item.Quantity;
                }
                else
                {
                    Remove(item);
                }
            }
            Guid number = Guid.NewGuid();
            CalculateTotal();
            var order = new Order() { TotalPrice = Total };
            order.Create(number, Products,userId);
            var email = new DeliverySystem();
            email.SendMessage(order.OrderedProducts,order.AmountsOrdered, order.OrderNumber);
            return order;
        }



    }
}
