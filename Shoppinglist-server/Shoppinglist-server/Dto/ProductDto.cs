namespace Shoppinglist_server.Dto
{
    public class ProductDto
    {
        public int ProductID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        
    }

}
