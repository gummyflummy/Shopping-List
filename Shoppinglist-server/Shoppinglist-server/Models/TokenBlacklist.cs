using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shoppinglist_server.Models
{
    public class TokenBlacklist
    {

        [Key]
        public string Token { get; set; } = string.Empty;
        public bool IsBlacklisted { get; set; } = true;
         

        public DateTime Expiration { get; set; }

        [ForeignKey("User")] 
        public int UserID { get; set; }

       




    }

}
