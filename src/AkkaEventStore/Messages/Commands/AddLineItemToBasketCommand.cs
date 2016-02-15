using AkkaEventStore.Actors;
using AkkaEventStore.Models;

namespace AkkaEventStore.Messages.Commands
{
    public class AddLineItemToBasketCommand : ICommand
    {
        public LineItem LineItem { get; private set; }

        public AddLineItemToBasketCommand(LineItem lineItem)
        {
            LineItem = lineItem;
        }

        // execute our command based on current state
        public bool Execute(IActorState state)
        {
            // Validate and do side effects
            if ((state as BasketActorState).basket.LineItems.Count > 100) return false;

            // success
            return true;
        }
    }
}
