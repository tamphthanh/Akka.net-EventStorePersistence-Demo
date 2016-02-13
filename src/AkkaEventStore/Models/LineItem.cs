using System;

namespace AkkaEventStore.Models
{
    public class LineItem
    {
        public LineItem()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
    }
}