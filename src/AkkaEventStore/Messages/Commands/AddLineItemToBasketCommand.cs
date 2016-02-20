using System;
using AkkaEventStore.Actors;
using AkkaEventStore.Models;

namespace AkkaEventStore.Messages.Commands
{
    public class AddLineItemToBasketCommand : ICommand
    {
        public LineItem LineItem { get; }

        public AddLineItemToBasketCommand(LineItem lineItem)
        {
            LineItem = lineItem;
        }
    }
}
