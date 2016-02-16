using AkkaEventStore.Models;
using System.Linq;

namespace AkkaEventStore.Messages.Events
{
    public class RemovedLineItemFromBasketEvent : IEvent<Basket>
    {
        public LineItem LineItem { get; private set; }

        public RemovedLineItemFromBasketEvent(LineItem lineItem)
        {
            LineItem = lineItem;
        }

        public Basket Apply(Basket basket)
        {
            var _basket = basket;
            _basket.LineItems.RemoveAll(li => li.Id == LineItem.Id); // TODO improve efficiency
            return _basket;
        }
    }
}
