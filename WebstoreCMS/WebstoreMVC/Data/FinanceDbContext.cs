using FinanceLibrary;
using Microsoft.EntityFrameworkCore;
using ShoppingCartLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebstoreMVC.Data
{
    public class FinanceDbContext: DbContext
    {
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
        {
           
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<PayAccount> PayAccounts { get; set; }
    }
}
