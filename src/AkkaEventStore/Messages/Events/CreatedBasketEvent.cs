using AkkaEventStore.Models;

namespace AkkaEventStore.Messages.Events
{
    public class CreatedBasketEvent : IEvent
    {
        public Basket Basket { get; }

        public CreatedBasketEvent(Basket basket)
        {
            Basket = basket;
        }
    }
}
