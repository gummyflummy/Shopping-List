using System.ComponentModel.DataAnnotations;

namespace Shoppinglist_server.Models
{
    public class ShoppingListDto
    {
        [Required]
        public int ListID { get; set; }

        [Required]
        public int ProductID { get; set; }

        public DateTime AddedDate { get; set; } = DateTime.UtcNow; 
    }
}
