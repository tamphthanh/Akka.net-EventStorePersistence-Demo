using AkkaEventStore.Models;
using System.Linq;

namespace AkkaEventStore.Messages.Events
{
    public class RemovedLineItemFromBasketEvent : IEvent<Basket>
    {
        public LineItem LineItem { get; }

        public RemovedLineItemFromBasketEvent(LineItem lineItem)
        {
            LineItem = lineItem;
        }

        public Basket Apply(Basket basket)
        {
            var newBasket = basket;
            basket.LineItems = basket.LineItems.Where(li => li.Id != LineItem.Id).ToList();
            return newBasket;
        }
    }
}
