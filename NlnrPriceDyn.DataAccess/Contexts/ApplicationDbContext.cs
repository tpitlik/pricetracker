using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NlnrPriceDyn.DataAccess.Common.Models;
using NlnrPriceDyn.DataAccess.Common.Models.ProductManagement;

namespace NlnrPriceDyn.DataAccess.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<UserDB>
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public virtual DbSet<ProductDB> Products { get; set; }
        public virtual DbSet<UserProductDB> UsersProducts { get; set; }
        public virtual DbSet<ProductPriceInfoDB> ProductPriceInfos { get; set; }
        public virtual DbSet<ProductNotificationDB> ProductNotifications { get; set; }
    }
}
