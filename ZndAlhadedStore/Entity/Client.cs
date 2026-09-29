using System.ComponentModel.DataAnnotations;

namespace ZndAlhadedStore.Entity
{
    public class Client
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public ICollection<Quote> Quotes { get; set; }
    }
}
