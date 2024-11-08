using System;
using Microsoft.EntityFrameworkCore;
using Shoppinglist_server;
using MySql.EntityFrameworkCore;
using Shoppinglist_server.Models;
using System.ComponentModel.DataAnnotations;



namespace Shoppinglist_server.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

    }
}