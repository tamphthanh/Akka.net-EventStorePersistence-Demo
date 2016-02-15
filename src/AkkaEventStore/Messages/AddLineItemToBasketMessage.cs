using AkkaEventStore.Models;

namespace AkkaEventStore.Messages
{
    public class AddLineItemToBasketMessage
    {
        public LineItem LineItem { get; private set; }
        public string BasketId { get; private set; }

        public AddLineItemToBasketMessage(string basketId, LineItem lineItem)
        {
            BasketId = basketId;
            LineItem = lineItem;
        }
    }
}
