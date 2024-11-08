using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppinglist_server;
using Shoppinglist_server.Models;

namespace Shoppinglist_server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ListsController : ControllerBase
    {
        private readonly ShoppingListDbContext _context;

        public ListsController(ShoppingListDbContext context)
        {
            _context = context;
        }

        // GET: api/lists
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<List>>> GetLists()
        {
            var userIdClaim = User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            int userId = int.Parse(userIdClaim.Value);

            
            var userLists = await _context.List
                .Where(l => l.UserID == userId)
                .ToListAsync();

            

            return userLists;
        }

        // GET: api/lists/5
       
        [HttpGet("{id}")]
        public async Task<ActionResult<List>> GetList(int id)
        {
            var userIdClaim = User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            int userId = int.Parse(userIdClaim.Value);

            var list = await _context.List
                .FirstOrDefaultAsync(l => l.ListID == id && l.UserID == userId);  

            if (list == null)
            {
                return NotFound("List not found or you do not have permission to view this list.");
            }

            return list;
        }

        // POST: api/lists
     
        [HttpPost]
        public async Task<ActionResult<List>> PostList(List list)
        {
            var userIdClaim = User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            
            list.UserID = int.Parse(userIdClaim.Value);

           
            _context.List.Add(list);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetList), new { id = list.ListID }, list);
        }

        // PUT: api/lists/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutList(int id, List list)
        {
            if (id != list.ListID)
            {
                return BadRequest();
            }

            _context.Entry(list).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ListExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/lists/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteList(int id)
        {
            var userIdClaim = User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            int userId = int.Parse(userIdClaim.Value);

          
            var list = await _context.List
                .FirstOrDefaultAsync(l => l.ListID == id && l.UserID == userId);  

            if (list == null)
            {
                return NotFound("List not found or you do not have permission to delete this list.");
            }

            _context.List.Remove(list);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ListExists(int id)
        {
            return _context.List.Any(e => e.ListID == id);
        }
    }
}
