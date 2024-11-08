using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shoppinglist_server.Dto;
using Shoppinglist_server.Models;

namespace Shoppinglist_server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShoppingListDbContext _context;

        public ProductsController(ShoppingListDbContext context)
        {
            _context = context;
        }
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var permittedImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!permittedImageTypes.Contains(file.ContentType))
            {
                return BadRequest("Invalid file type. Only image files (JPEG, PNG, GIF, WebP) are allowed. If you try to upload any other file Type again, an Agent will move to eliminate you");
            }

            Console.WriteLine($"File received: {file.FileName}, Size: {file.Length}");

          
            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            var filePath = Path.Combine(uploadDir, file.FileName);
            Console.WriteLine($"Saving file to: {filePath}");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imagePath = $"/images/{file.FileName}";
            return Ok(new { imagePath });
        }


        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _context.Product.ToListAsync();
            return products.Select(product => new ProductDto
            {
                ProductID = product.ProductID,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                Unit = product.Unit,
                ImagePath = product.ImagePath
            }).ToList();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            Console.WriteLine($"Fetched product details: ID = {product.ProductID}, Name = {product.Name}");
            return new ProductDto
            {
                ProductID = product.ProductID,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                Unit = product.Unit,
                ImagePath = product.ImagePath
            };
        }

        // PUT: api/Products
        [HttpPut("{id}")] 
        public async Task<IActionResult> PutProduct(int id, ProductDto productDto)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

           
            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.Quantity = productDto.Quantity;
            product.Unit = productDto.Unit;
            product.ImagePath = productDto.ImagePath;

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        
        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<ProductDto>> PostProduct([FromBody] ProductDto productDto)
        {
          
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Quantity = productDto.Quantity,
                Unit = productDto.Unit,
                ImagePath = productDto.ImagePath 
            };

            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductID }, new ProductDto
            {
                ProductID = product.ProductID,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                Unit = product.Unit,
                ImagePath = product.ImagePath
            });
        }


        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductID == id);
        }
    }
}
