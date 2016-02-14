using Akka.Actor;
using Akka.Persistence;
using AkkaEventStore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AkkaEventStore.Actors
{
    public class IncrementBasketIdCommand
    {

    }

    public class IncrementBasketIdEvent : IBasketCoordinatorEvent
    {
        public int Apply(int counter)
        {
            return counter + 1;
        }
    }

    public class AddLineItemToSpecificBasketCommand
    {
        public LineItem LineItem { get; private set; }
        public string BasketId { get; private set; }

        public AddLineItemToSpecificBasketCommand(string basketId, LineItem lineItem)
        {
            BasketId = basketId;
            LineItem = lineItem;
        }
    }

    public interface IBasketCoordinatorEvent
    {
        int Apply(int counter);
    }

    public class BasketCoordinatorActorState : IActorState
    {
        public int counter;

        public BasketCoordinatorActorState Update(IBasketCoordinatorEvent evt)
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

        public void UpdateState(IBasketCoordinatorEvent evt)
        {
            State = (State as BasketCoordinatorActorState).Update(evt);
            var basketId = "basket-" + (State as BasketCoordinatorActorState).counter;
            baskets.Add(basketId, Context.ActorOf(Props.Create<BasketActor>(basketId), basketId));
        }

        protected override bool ReceiveRecover(object message)
        {
            BasketCoordinatorActorState state;

            if (message is IBasketCoordinatorEvent)
            {
                UpdateState(message as IBasketCoordinatorEvent);
            }
            else if (message is SnapshotOffer && (state = ((SnapshotOffer)message).Snapshot as BasketCoordinatorActorState) != null)
                State = state;
            else return false;
            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            base.ReceiveCommand(message);

            if (message is IncrementBasketIdCommand)
            {
                Persist(new IncrementBasketIdEvent(), UpdateState);
            }
            else if (message is AddLineItemToSpecificBasketCommand)
            {
                var cmd = (message as AddLineItemToSpecificBasketCommand);
                if (baskets.ContainsKey(cmd.BasketId))
                {
                    baskets[cmd.BasketId].Forward(new AddLineItemToBasketCommand(cmd.LineItem));
                }
                else
                {
                    Console.WriteLine("No such basket");
                }
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
