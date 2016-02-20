using AkkaEventStore.Actors;
using AkkaEventStore.Models;

namespace AkkaEventStore.Messages.Commands
{
    public class RemoveLineItemFromBasketCommand : ICommand
    {
        public LineItem LineItem { get; }

        public RemoveLineItemFromBasketCommand(LineItem lineItem)
        {
            LineItem = lineItem;
        }
    }
}
