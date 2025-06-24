using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineStoreWebAPI.Model;

namespace OnlineStoreWebAPI.DBContext
{
    public class OnlineStoreDBContext: DbContext
    {
        public OnlineStoreDBContext
            (DbContextOptions<OnlineStoreDBContext> options)
            : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.orders)
                .HasForeignKey(o => o.userId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.orderItems)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.orderItems)
                .HasForeignKey(oi => oi.productId);
            

            base.OnModelCreating(modelBuilder);
        }


    }
}
