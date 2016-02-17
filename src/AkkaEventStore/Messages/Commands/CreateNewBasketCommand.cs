using System;
using AkkaEventStore.Actors;

namespace AkkaEventStore.Messages.Commands
{
    public struct CreateNewBasketCommand : ICommand
    {
        public bool Execute(IActorState state)
        {
            throw new NotImplementedException();
        }
    }
}
