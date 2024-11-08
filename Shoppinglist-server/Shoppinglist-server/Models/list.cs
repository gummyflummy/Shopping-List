using System;
using Microsoft.EntityFrameworkCore;
using Shoppinglist_server;
using MySql.EntityFrameworkCore;
using Shoppinglist_server.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shoppinglist_server.Models
{
    public class List
    {
        [Key]
        public int ListID { get; set; }

        [ForeignKey("User")] 
        public int UserID { get; set; }

        public DateTime CreatedDate { get; set; }

       
    }
}