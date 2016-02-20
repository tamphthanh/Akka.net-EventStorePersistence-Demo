using Akka.Persistence;
using System;

namespace AkkaEventStore.Actors
{
    public abstract class PersistentStateActor : PersistentActor
    {
        public abstract IActorState State { get; set; }

        protected override bool ReceiveCommand(object message)
        {
            if (message as string == "peek") Console.WriteLine(State);
            return true;
        }
    }
}
