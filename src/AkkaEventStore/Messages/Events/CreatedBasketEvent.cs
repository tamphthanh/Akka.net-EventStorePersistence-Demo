using AkkaEventStore.Models;

namespace AkkaEventStore.Messages.Events
{
    public class CreatedBasketEvent : IEvent<Basket>
    {
        public Basket Basket { get; }

        public CreatedBasketEvent(Basket basket)
        {
            Basket = basket;
        }

        // passed basket will be empty since we are dealing with initial event
        // return serialized basket in case it had some initial data
        public Basket Apply(Basket basket) => Basket;        
    }
}
