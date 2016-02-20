using AkkaEventStore.Actors;
using AkkaEventStore.Actors.Messages.Commands;
using AkkaEventStore.Messages.Commands;
using System;

namespace AkkaEventStore.Messages.Handlers.Basket
{
    public static class BasketCommandHandlers
    {
        public static bool Handle(IActorState state, ICommand command)
        {
            if (command is CreateBasketCommand)
            {
                return true;
            }
            else if (command is AddLineItemToBasketCommand)
            {
                // Validate and do side effects
                if ((state as BasketActorState).basket.LineItems.Count > 100) return false;

                // success
                return true;
            }
            else if (command is RemoveLineItemFromBasketCommand)
            {
                return (state as BasketActorState).basket
                    .LineItems
                    .Exists(li => li.Id == (command as RemoveLineItemFromBasketCommand).LineItem.Id);
            }
            throw new NotImplementedException($"Unable to handle {command}");
        }        
    }
}
