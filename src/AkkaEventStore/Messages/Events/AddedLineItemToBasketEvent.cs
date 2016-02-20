using AkkaEventStore.Models;

namespace AkkaEventStore.Messages.Events
{
    public class AddedLineItemToBasketEvent : IEvent<Basket>
    {
        public LineItem LineItem { get; }

        public AddedLineItemToBasketEvent(LineItem lineItem)
        {
            LineItem = lineItem;
        }

        public Basket Apply(Basket basket)
        {
            var newBasket = basket;
            newBasket.LineItems.Add(LineItem);
            return newBasket;
        }
    }
}
