using AkkaEventStore.Models;

namespace AkkaEventStore.Messages.Events
{
    public class CreatedBasketEvent : IEvent<Basket>
    {
        public Basket Basket { get; private set; }

        public CreatedBasketEvent(Basket basket)
        {
            Basket = basket;
        }

        public Basket Apply(Basket basket) // passed basket will be empty since we are dealing with initial event
        {
            return Basket; // return serialized basket in case it had some initial data
        }
    }
}
