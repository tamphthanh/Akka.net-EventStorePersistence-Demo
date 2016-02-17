using AkkaEventStore.Messages.Commands;
using AkkaEventStore.Models;

namespace AkkaEventStore.Actors.Messages.Commands
{
    public struct CreateBasketCommand : ICommand
    {
        public Basket basket { get; private set; }

        public CreateBasketCommand(string id)
        {
            basket = new Basket { Id = id };
        }

        public bool Execute(IActorState state)
        {
            return true;
        }
    }
}
