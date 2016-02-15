using System;
using AkkaEventStore.Models;

namespace AkkaEventStore.Messages.Events
{
    public class CreatedBasketEvent : IEvent
    {
        public Basket Basket { get; private set; }

        public CreatedBasketEvent(Basket basket)
        {
            Basket = basket;
        }

        public override string ToString()
        {
            return Basket.Id;
        }

        public Basket Apply(Basket basket)
        {
            return basket;
        }
    }
}
