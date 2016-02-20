using AkkaEventStore.Models;

namespace AkkaEventStore.Messages.Events
{
    public class RemovedLineItemFromBasketEvent : IEvent
    {
        public LineItem LineItem { get; }

        public RemovedLineItemFromBasketEvent(LineItem lineItem)
        {
            LineItem = lineItem;
        }
    }
}
