using Akka.Actor;
using Akka.Persistence;
using AkkaEventStore.Actors.Messages.Commands;
using AkkaEventStore.Messages;
using AkkaEventStore.Messages.Commands;
using AkkaEventStore.Messages.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AkkaEventStore.Actors
{
    public class BasketCoordinatorActorState : IActorState
    {
        public int counter = 1;

        public BasketCoordinatorActorState Update(IEvent<int> evt)
        {
            return new BasketCoordinatorActorState { counter = evt.Apply(counter) };
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(counter, Formatting.Indented);
        }
    }

    public class BasketCoordinatorActor : PersistentStateActor
    {
        public override string PersistenceId { get; }
        public override IActorState State { get; set; }
        private IDictionary<string, IActorRef> baskets = new Dictionary<string, IActorRef>();

        public BasketCoordinatorActor()
        {
            PersistenceId = "BasketCoordinatorActor";
            State = new BasketCoordinatorActorState();
        }

        public void UpdateState(IEvent<int> evt)
        {
            if (IsRecovering)
            {
                var basketId = "basket-" + (State as BasketCoordinatorActorState).counter;
                baskets.Add(basketId, Context.ActorOf(Props.Create<BasketActor>(basketId), basketId));
            }
            State = (State as BasketCoordinatorActorState).Update(evt);
        }

        protected override bool ReceiveRecover(object message)
        {
            BasketCoordinatorActorState state;

            if (message is IEvent<int>)
            {
                UpdateState(message as IEvent<int>);
            }
            else if (message is SnapshotOffer && (state = ((SnapshotOffer)message).Snapshot as BasketCoordinatorActorState) != null)
                State = state;
            else if (message is RecoveryCompleted)
                Console.WriteLine($"{PersistenceId} Recovery Completed.");            
            else return false;
            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            base.ReceiveCommand(message);

            if (message is CreateNewBasketCommand)
            {
                var basketId = "basket-" + (State as BasketCoordinatorActorState).counter;
                baskets.Add(basketId, Context.ActorOf(Props.Create<BasketActor>(basketId), basketId));
                var success = (bool)baskets[basketId].Ask(new CreateBasketCommand(basketId)).Result;
                if (success) Persist(new CreateNewBasketEvent(), UpdateState);
            }
            else if (message is AddLineItemToBasketMessage)
            {
                var cmd = (message as AddLineItemToBasketMessage);
                if (baskets.ContainsKey(cmd.BasketId))                
                    baskets[cmd.BasketId].Forward(new AddLineItemToBasketCommand(cmd.LineItem));                
                else                
                    Console.WriteLine("No such basket");                
            }
            else if (message is RemoveLineItemFromBasketMessage)
            {
                var cmd = (message as RemoveLineItemFromBasketMessage);
                if (baskets.ContainsKey(cmd.BasketId))
                    baskets[cmd.BasketId].Forward(new RemoveLineItemFromBasketCommand(cmd.LineItem));
                else
                    Console.WriteLine("No such basket");
            }
            else if ((message as string).StartsWith("peekBasket "))
            {
                var tokens = (message as string).Split(' ');
                var basketId = tokens[1];
                if (baskets.ContainsKey(basketId))
                {
                    baskets[basketId].Tell("peek");
                }
                else
                {
                    Console.WriteLine("No such basket");
                }
            }
            else return false;
            return true;
        }
    }
}
