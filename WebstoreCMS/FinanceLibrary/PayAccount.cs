using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text;

namespace FinanceLibrary
{
    public class PayAccount
    {
        //Properties
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        [StringLength(10, MinimumLength = 4)]
        public string IbanNumber { get; set; }
        [Required]
        public string Money { get; set; }

        //Constructor
        public PayAccount()
        {
            Money = "100000";
        }

        //Methods
        public void Payment(PayAccount customer, PayAccount storeowner, Order order)
        {
            if (order.Status == true)
            {
                decimal.TryParse(order.TotalPrice, out decimal price);
                customer.PayPayment(price);
                storeowner.RecievePayment(price);
                //SaveChanges In Database
            }
            else if (order.Status == false)
            {
                decimal.TryParse(order.TotalPrice, out decimal price);
                customer.RecievePayment(price);
                storeowner.PayPayment(price);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public void PayPayment(decimal price)
        {
            decimal.TryParse(Money, out decimal money);
            money -= price;
            Money = money.ToString();
        }

        public void RecievePayment(decimal price)
        {
            decimal.TryParse(Money, out decimal money);
            money += price;
            Money = money.ToString();
        }
    }
}
