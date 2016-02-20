using AkkaEventStore.Models;

namespace AkkaEventStore.Messages.Events
{
    public class AddedLineItemToBasketEvent : IEvent
    {
        public LineItem LineItem { get; }

        public AddedLineItemToBasketEvent(LineItem lineItem)
        {
            LineItem = lineItem;
        }
    }
}
