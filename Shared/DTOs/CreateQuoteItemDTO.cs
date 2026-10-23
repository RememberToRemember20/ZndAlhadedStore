using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace Shared.DTOs
{
    public class CreateQuoteItemDTO
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal Discount { get; set; }

    }
}
