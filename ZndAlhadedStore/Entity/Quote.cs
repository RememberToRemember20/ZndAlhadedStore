using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZndAlhadedStore.Entity
{
    public class Quote
    {
        [Key]
       public int Id { get; set; }
        public DateTime CreatedAt { get;set;  }
        [Column(TypeName = "decimal(18,4)")]
        public decimal ExchangeRate {  get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalSYP { get; set;  }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalUSD { get; set;  }
        public int TotalQuantity { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }  
        public ICollection<QuoteItem> QuoteItems { get; set; }

    }
}
