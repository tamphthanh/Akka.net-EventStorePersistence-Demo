using AkkaEventStore.Models;

namespace AkkaEventStore.Messages
{
    public struct RemoveLineItemFromBasketMessage
    {
        public LineItem LineItem { get; }
        public string BasketId { get; }

        public RemoveLineItemFromBasketMessage(string basketId, LineItem lineItem)
        {
            BasketId = basketId;
            LineItem = lineItem;
        }
    }
}
