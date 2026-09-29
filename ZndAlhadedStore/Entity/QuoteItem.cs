using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZndAlhadedStore.Entity
{
    public class QuoteItem
    {
        [Key]
        public int Id { get; set; }
        public string ProductName{ get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal ProductPriceSYP{ get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal ProductPriceUSD { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId{ get; set; }
        public Product Product { get; set; }
        [ForeignKey(nameof(Quote))]
        public int QuoteId { get; set; }
        public Quote Quote { get; set; }
        public int Count { get; set; }

    }
}
