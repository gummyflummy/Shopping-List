using System;
using Microsoft.EntityFrameworkCore;
using Shoppinglist_server;
using MySql.EntityFrameworkCore;
using Shoppinglist_server.Models;



namespace Shoppinglist_server.Models
{
    public class UserDto
    {
        public required string Username { get; set; } 
        public required string Password { get; set; } 
    }
}