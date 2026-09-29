using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class UpdateProductDTO
    {
        public string? Name { get; set; }
        public string? ArabicDescription { get; set; }
        public string? EnglishDescription { get; set; }
        public decimal? Price { get; set; }
        public Currency? Currency { get; set; }
    }
}
