using System;

namespace AkkaEventStore.Models
{
    public class LineItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
    }
}