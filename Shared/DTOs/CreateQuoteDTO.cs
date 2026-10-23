using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Shared.DTOs
{
    public class CreateQuoteDTO
    {
        public decimal ExchangeRate { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal Discount { get; set; }
        public int ClientId { get; set; }
        public ICollection<CreateQuoteItemDTO> QuoteItems { get; set; }
    }
}
