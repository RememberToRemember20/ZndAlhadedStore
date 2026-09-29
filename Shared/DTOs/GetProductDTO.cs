using System.ComponentModel.DataAnnotations.Schema;


namespace Shared.DTOs
{
    public class GetProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ArabicDescription { get; set; }
        public string? EnglishDescription { get; set; }
       
        public decimal Price { get; set; }

        public Currency Currency { get; set; }
        public string ImageURL { get; set; }
     
    }
}
