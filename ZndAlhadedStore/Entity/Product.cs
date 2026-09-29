using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.DTOs;

namespace ZndAlhadedStore.Entity
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ArabicDescription { get; set; }
        public string? EnglishDescription { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public Currency Currency { get; set; }
        public string? ImageURL { get; set; }

    }
}
