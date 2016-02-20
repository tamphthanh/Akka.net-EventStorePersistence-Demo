using AkkaEventStore.Models;

namespace AkkaEventStore.Messages
{
    public struct AddLineItemToBasketMessage
    {
        public LineItem LineItem { get; }
        public string BasketId { get; }

        public AddLineItemToBasketMessage(string basketId, LineItem lineItem)
        {
            BasketId = basketId;
            LineItem = lineItem;
        }
    }
}
