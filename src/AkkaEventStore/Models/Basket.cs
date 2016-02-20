using System.Collections.Generic;

namespace AkkaEventStore.Models
{
    public class Basket
    {
        public string Id { get; set; }
        public List<LineItem> LineItems { get; set; } = new List<LineItem>();
        public Address Address { get; set; }
    }
}
