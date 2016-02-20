using AkkaEventStore.Messages.Commands;
using AkkaEventStore.Models;

namespace AkkaEventStore.Actors.Messages.Commands
{
    public class CreateBasketCommand : ICommand
    {
        public Basket basket { get; }

        public CreateBasketCommand(string id)
        {
            basket = new Basket { Id = id };
        }        
    }
}
