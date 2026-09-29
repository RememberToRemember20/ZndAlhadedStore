using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Shared.DTOs
{
    public class GetQuoteDTO
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal TotalSYP { get; set; }
        public decimal TotalUSD { get; set; }
        public int TotalQuantity { get; set; }
        public int ClientId { get; set; }
        public GetClientDTO Client { get; set; }
        public ICollection<GetQuoteItemDTO> QuoteItems { get; set; }
    }
}
