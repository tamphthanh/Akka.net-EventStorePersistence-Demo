using Akka.Persistence;
using AkkaEventStore.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
