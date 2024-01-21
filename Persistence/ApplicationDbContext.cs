using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*modelBuilder.Entity<Actor_Movie>().HasKey(i => new
            {
                i.MovieId,
                i.ActorId
            });

            modelBuilder.Entity<Actor_Movie>()
                .HasOne(t => t.Movie)
                .WithMany(y => y.Actor_Movie)
                .HasForeignKey(u => u.MovieId);

            modelBuilder.Entity<Actor_Movie>()
                .HasOne(i => i.Actor)
                .WithMany(t => t.Actor_Movie)
                .HasForeignKey(o => o.ActorId);*/

            modelBuilder.Entity<PlaceOrderItems>(b =>
            {
                b.HasOne(o => o.Vendor)
                    .WithMany(f => f.Vendors)
                    .HasForeignKey(o => o.VendorId);
            });
            modelBuilder.Entity<Voucher>(b =>
            {
                b.HasOne(o => o.Movie)
                    .WithMany(f => f.Vouchers)
                    .HasForeignKey(o => o.MovieId);
            });

            modelBuilder.Entity<MovieUserView>(b =>
            {
                b.HasOne(o => o.Movie)
                    .WithMany(f => f.MovieUserViews)
                    .HasForeignKey(o => o.MovieId);
            });

            modelBuilder.Entity<ReturnProductImage>(b =>
            {
                b.HasOne(o => o.ReturnProduct)
                    .WithMany(f => f.ReturnProductImages)
                    .HasForeignKey(o => o.ReturnProductId);
            });
 
            /* .WithRequired(x => x.Master)
             .WillCascadeOnDelete(true);*/

            modelBuilder.Entity<VoucherNotification>(b =>
            {
                b.HasOne(o => o.Voucher)
                    .WithMany(f => f.VoucherNotifications)
                    .HasForeignKey(o => o.VoucherId)
                     .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<ShoppingVoucher>(b =>
            {
                b.HasOne(o => o.ShoppingCartItem)
                    .WithMany(f => f.ShoppingVouchers)
                    .HasForeignKey(o => o.ShoppingCartItemId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<ShoppingVoucher>(b =>
            {
                b.HasOne(o => o.Voucher)
                    .WithMany(f => f.ShoppingVouchers)
                    .HasForeignKey(o => o.VoucherId);
            });

            modelBuilder.Entity<ShoppingVoucher>()
               .HasOne(a => a.ShoppingCartItem)
               .WithMany(c => c.ShoppingVouchers)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MessageUsersTable>()
                .HasOne(a => a.MessageTable)
                .WithMany(c => c.MessageUsersTables)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DiscountShop_Cart>()
               .HasOne(a => a.ShoppingCartItem)
               .WithMany(c => c.DiscountShop_Carts)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DiscountShop_Cart>()
               .HasOne(a => a.DiscountedShop)
               .WithMany(c => c.DiscountShop_Carts)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MovieOutOfStockNotification>()
             .HasOne(a => a.MovieOutOfStock)
             .WithMany(c => c.MovieOutOfStockNotifications)
             .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Movie> Movies { get; set; }
        /*public DbSet<Actor> Actors { get; set; }
        public DbSet<Actor_Movie> Actors_Movies { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Producer> Producers { get; set; }*/
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
        public DbSet<PlaceOrderItems> PlaceOrderItems { get; set; }
        public DbSet<RateProduct> RateProducts { get; set; }
        public DbSet<ProductTransaction> ProductTransactions { get; set; }
        public DbSet<ProductTransactionResponse> ProductTransactionResponses { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<MovieUserView> MovieUserViews { get; set; }
        public DbSet<ReturnProduct> ReturnProducts { get; set; }
        public DbSet<ReturnReasons> ReturnReasons { get; set; }
        public DbSet<ReturnProductImage> ReturnProductImages { get; set; }
        public DbSet<ProductTransactionNotif> ProductTransactionNotif { get; set; }
        public DbSet<VoucherNotification> VoucherNotifications { get; set; }
        public DbSet<ReturnProductNotification> ReturnProductNotifications { get; set; }
        public DbSet<ShoppingVoucher> ShoppingVouchers { get; set; }
        public DbSet<MessageTable> MessageTables { get; set; }
        public DbSet<MessageUsersTable> MessageUsersTables { get; set; }
        public DbSet<MessageNotification> MessageNotifications { get; set; }
        // Vendor out of stock product notif 
        public DbSet<MovieNotification> MovieNotifications { get; set; }
        public DbSet<DiscountedShop> DiscountedShop { get; set; }
        public DbSet<DiscountShop_Cart> DiscountShop_Cart { get; set; }

        public DbSet<MovieOutOfStock> MovieOutOfStocks { get; set; }
        public DbSet<MovieOutOfStockNotification> MovieOutOfStockNotifications { get; set; }

    }
}
