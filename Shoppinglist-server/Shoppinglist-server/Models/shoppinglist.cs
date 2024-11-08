using System;
using Microsoft.EntityFrameworkCore;
using Shoppinglist_server;
using MySql.EntityFrameworkCore;
using Shoppinglist_server.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shoppinglist_server.Models
{
    public class ShoppingList
    {
        [Key, Column(Order = 0), ForeignKey("ListID")]
        public int ListID { get; set; }

        [Key, Column(Order = 1), ForeignKey("ProductID")]
        public int ProductID { get; set; }

        public DateTime AddedDate { get; set; }

   
       
    }
}

