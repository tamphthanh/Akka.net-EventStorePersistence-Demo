using System.Collections.Generic;

namespace AkkaEventStore.Models
{
    public class Basket
    {
        public Basket()
        {
            LineItems = new List<LineItem>();
        }

        public string Id { get; set; }
        public List<LineItem> LineItems { get; set; }
        public Address Address { get; set; }
    }
}
