

namespace Shared.DTOs
{
    public class GetQuoteItemDTO
    {
        
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPriceSYP { get; set; }
        public decimal ProductPriceUSD { get; set; }
        public int ProductId { get; set; }
        public GetProductDTO Product { get; set; }
        public int QuoteId { get; set; }
        public GetQuoteDTO Quote { get; set; }
        public int Count { get; set; }

    }
}
