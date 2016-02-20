using AkkaEventStore.Actors;
using AkkaEventStore.Actors.Messages.Commands;
using AkkaEventStore.Messages.Commands;
using AkkaEventStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaEventStore.Messages.Handlers.Basket
{
    public static class BasketCommandHandlers
    {
        public static bool CreateBasketCommandHandler(IActorState state, CreateBasketCommand command) => true;

        public static bool AddLineItemToBasketCommandHandler(IActorState state, AddLineItemToBasketCommand command)
        {
            // Validate and do side effects
            if ((state as BasketActorState).basket.LineItems.Count > 100) return false;

            // success
            return true;
        }

        public static bool RemoveLineItemFromBasketCommand(IActorState state, RemoveLineItemFromBasketCommand command) =>
            (state as BasketActorState).basket
                .LineItems
                .Exists(li => li.Id == command.LineItem.Id);      
    }
}
