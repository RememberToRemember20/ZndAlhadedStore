using System.ComponentModel.DataAnnotations.Schema;


namespace Shared.DTOs
{
    public class CreateProductDTO
    {
        public string Name { get; set; }
        public string? ArabicDescription { get; set; }
        public string? EnglishDescription { get; set; }
      
        public decimal Price { get; set; }

        public Currency Currency { get; set; }
        
    }
}
