using AkkaEventStore.Actors;
using AkkaEventStore.Models;

namespace AkkaEventStore.Messages.Commands
{
    public class RemoveLineItemFromBasketCommand : ICommand
    {
        public LineItem LineItem { get; private set; }

        public RemoveLineItemFromBasketCommand(LineItem lineItem)
        {
            LineItem = lineItem;
        }

        public bool Execute(IActorState state)
        {
            var basket = (state as BasketActorState).basket;
            if (basket.LineItems.Exists(li => li.Id == LineItem.Id)) // do other validation here like product/price/quantity
                return true;         
            return false;
        }
    }
}
