using AkkaEventStore.Models;

namespace AkkaEventStore.Messages
{
    public class RemoveLineItemFromBasketMessage
    {
        public LineItem LineItem { get; private set; }
        public string BasketId { get; private set; }

        public RemoveLineItemFromBasketMessage(string basketId, LineItem lineItem)
        {
            BasketId = basketId;
            LineItem = lineItem;
        }
    }
}
