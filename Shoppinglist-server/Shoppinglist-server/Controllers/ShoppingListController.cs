using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppinglist_server.Models;
using System.Security.Claims;

namespace Shoppinglist_server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingListController : ControllerBase
    {
        private readonly ShoppingListDbContext _context;

        public ShoppingListController(ShoppingListDbContext context)
        {
            _context = context;
        }

        // GET: api/shoppinglist
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShoppingListDto>>> GetShoppingLists()
        {
            return await _context.ShoppingList
                .Select(sl => new ShoppingListDto
                {
                    ListID = sl.ListID,
                    ProductID = sl.ProductID,
                    AddedDate = sl.AddedDate
                }).ToListAsync();
        }

        // GET: api/shoppinglist/5
        [HttpGet("{listId}/{productId}")]
        public async Task<ActionResult<ShoppingListDto>> GetShoppingList(int listId, int productId)
        {
            var shoppingList = await _context.ShoppingList
                .FirstOrDefaultAsync(sl => sl.ListID == listId && sl.ProductID == productId);

            if (shoppingList == null)
            {
                return NotFound();
            }

            return new ShoppingListDto
            {
                ListID = shoppingList.ListID,
                ProductID = shoppingList.ProductID,
                AddedDate = shoppingList.AddedDate
            };
        }

        [HttpPost]
        public async Task<ActionResult<ShoppingListDto>> PostShoppingList([FromBody] ShoppingListDto shoppingListDto)
        {
            var userIdClaim = User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return BadRequest("User ID is missing from the token.");
            }

            int userId;
            if (!int.TryParse(userIdClaim.Value, out userId))
            {
                return BadRequest("Invalid User ID.");
            }

           
            var userShoppingList = await _context.List
                .FirstOrDefaultAsync(l => l.UserID == userId && l.ListID == shoppingListDto.ListID);

            if (userShoppingList == null)
            {
                return BadRequest("The specified list does not exist for this user.");
            }

           
            var shoppingListEntry = new ShoppingList
            {
                ListID = shoppingListDto.ListID, 
                ProductID = shoppingListDto.ProductID,
                AddedDate = DateTime.UtcNow
            };

            _context.ShoppingList.Add(shoppingListEntry);
            await _context.SaveChangesAsync();

            var resultDto = new ShoppingListDto
            {
                ListID = shoppingListEntry.ListID,
                ProductID = shoppingListEntry.ProductID,
                AddedDate = shoppingListEntry.AddedDate
            };

            return CreatedAtAction(nameof(PostShoppingList), resultDto);
        }





        // PUT: api/shoppinglist/5
        [HttpPut("{listId}/{productId}")]
        public async Task<IActionResult> PutShoppingList(int listId, int productId, ShoppingListDto shoppingListDto)
        {
            if (listId != shoppingListDto.ListID || productId != shoppingListDto.ProductID)
            {
                return BadRequest();
            }

            var shoppingList = new ShoppingList
            {
                ListID = shoppingListDto.ListID,
                ProductID = shoppingListDto.ProductID,
                AddedDate = shoppingListDto.AddedDate
            };

            _context.Entry(shoppingList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShoppingListExists(listId, productId))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/shoppinglist/5
        [HttpDelete("{listId}/{productId}")]
        public async Task<IActionResult> DeleteShoppingList(int listId, int productId)
        {
            var shoppingList = await _context.ShoppingList.FindAsync(listId, productId);
            if (shoppingList == null)
            {
                return NotFound();
            }

            _context.ShoppingList.Remove(shoppingList);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShoppingListExists(int listId, int productId)
        {
            return _context.ShoppingList.Any(e => e.ListID == listId && e.ProductID == productId);
        }
    }
}
