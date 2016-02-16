using AkkaEventStore.Actors;
using AkkaEventStore.Models;
using Newtonsoft.Json;

namespace AkkaEventStore.Messages.Events
{
    public class AddedLineItemToBasketEvent : IEvent<Basket>
    {
        public LineItem LineItem { get; private set; }

        public AddedLineItemToBasketEvent(LineItem lineItem)
        {
            LineItem = lineItem;
        }

        public Basket Apply(Basket basket)
        {
            var _basket = basket;
            _basket.LineItems.Add(LineItem);
            return _basket;
        }
    }
}
