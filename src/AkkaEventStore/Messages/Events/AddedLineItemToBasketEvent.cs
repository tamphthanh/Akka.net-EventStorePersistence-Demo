using AkkaEventStore.Models;
using Newtonsoft.Json;

namespace AkkaEventStore.Messages.Events
{
    public class AddedLineItemToBasketEvent : IEvent
    {
        private Basket _basket { get; set; }
        public LineItem LineItem { get; private set; }

        public AddedLineItemToBasketEvent(LineItem lineItem)
        {
            LineItem = lineItem;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(LineItem);
        }

        public Basket Apply(Basket basket)
        {
            _basket = basket;
            _basket.LineItems.Add(LineItem);
            return _basket;
        }
    }
}
