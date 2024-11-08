using System;
using Microsoft.EntityFrameworkCore;
using Shoppinglist_server;
using MySql.EntityFrameworkCore;
using Shoppinglist_server.Models;


namespace Shoppinglist_server
{
    public class ShoppingListDbContext : DbContext
    {
        public ShoppingListDbContext(DbContextOptions<ShoppingListDbContext> context):base(context) {
        }

        public DbSet<Product> Product { get; set; }
        public DbSet<List> List { get; set; }
        public DbSet<ShoppingList> ShoppingList { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<TokenBlacklist> TokenBlacklist { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShoppingList>()
                .HasKey(sl => new { sl.ListID, sl.ProductID }); 

            modelBuilder.Entity<TokenBlacklist>()
           .HasKey(t => t.Token);
        }
    }
}
