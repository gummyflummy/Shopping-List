using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.EntityFrameworkCore;
using Shoppinglist_server;
using MySql.EntityFrameworkCore;
using Shoppinglist_server.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;


namespace Shoppinglist_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ShoppingListDbContext _context;
        private readonly IConfiguration _configuration;
        
        public AuthController(IConfiguration configuration, ShoppingListDbContext context)
        {
            _context = context;
            _configuration = configuration;
        }
      
       



         [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            if (await _context.User.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest("User already exists.");
            }

            var user = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(UserDto request)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("Wrong password");
            }


            string token = CreateToken(user);

            var existingToken = await _context.TokenBlacklist.FirstOrDefaultAsync(t => t.Token == token);
            if (existingToken != null)
            {
                _context.TokenBlacklist.Remove(existingToken); 
                await _context.SaveChangesAsync(); 
            }

            var tokenBlacklist = new TokenBlacklist
            {
                Token = token,
                IsBlacklisted = false,
                Expiration = DateTime.UtcNow.AddDays(1), 
                UserID= user.UserID

            };
            _context.TokenBlacklist.Add(tokenBlacklist);
            await _context.SaveChangesAsync();

            return Ok(token);

        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing.");
            }

            var existingToken = await _context.TokenBlacklist.FirstOrDefaultAsync(t => t.Token == token);

            if (existingToken != null)
            {
                existingToken.IsBlacklisted = true;
            }
            else
            {
                var tokenBlacklist = new TokenBlacklist { Token = token, IsBlacklisted = true };
                _context.TokenBlacklist.Add(tokenBlacklist);
            }

            await _context.SaveChangesAsync();

            return Ok("Token blacklisted successfully.");
        }


        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("UserID", user.UserID.ToString())
                 
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("Jwt:Key").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var expiration = DateTime.UtcNow.AddDays(1);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds

            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            var tokenBlacklist = new TokenBlacklist
            {
                Token = jwt,
                IsBlacklisted = false,
                Expiration = expiration,
                UserID = user.UserID 
            };

            _context.TokenBlacklist.Add(tokenBlacklist);
            _context.SaveChanges();

            return jwt;
        }
    }
}
