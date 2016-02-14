using Akka.Persistence;
using AkkaEventStore.Models;
using Newtonsoft.Json;
using System;

namespace AkkaEventStore.Actors
{
    #region Commands
    public class CreateBasketCommand
    {
        public Basket basket { get; private set; }

        public CreateBasketCommand(string id)
        {
            basket = new Basket { Id = id };
        }
    }

    public class AddLineItemToBasketCommand
    {
        public LineItem LineItem { get; private set; }

        public AddLineItemToBasketCommand(LineItem lineItem)
        {
            LineItem = lineItem;
        }
    }
    #endregion

    #region Events
    public interface IEvent
    {
        Basket Apply(Basket basket);
    }

    // this is weird
    public class CreatedBasketEvent : IEvent
    {
        public Basket basket { get; private set; }

        public CreatedBasketEvent(string id)
        {
            basket = new Basket { Id = id };
        }

        public override string ToString()
        {
            return basket.Id;
        }

        public Basket Apply(Basket basket)
        {
            return basket;
        }
    }

    public class AddedLineItemToBasketEvent : IEvent
    {
        private Basket _basket { get; set; }
        public LineItem _lineItem { get; private set; }

        public AddedLineItemToBasketEvent(LineItem lineItem)
        {
            _lineItem = lineItem;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(_lineItem);
        }

        public Basket Apply(Basket basket)
        {
            _basket = basket;
            _basket.LineItems.Add(_lineItem);
            return _basket;
        }
    }
    #endregion

    public class ActorState : IActorState
    {
        public Basket basket = new Basket();

        public ActorState Update(IEvent evt)
        {
            return new ActorState { basket = evt.Apply(basket) };
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(basket, Formatting.Indented);
        }
    }

    public class BasketActor : PersistentStateActor
    {
        public override string PersistenceId { get; }

        public override IActorState State { get; set; }

        public BasketActor(string id)
        {
            State = new ActorState();
            (State as ActorState).basket.Id = id;
            PersistenceId = id;
        }

        public void UpdateState(IEvent evt)
        {
            State =  (State as ActorState).Update(evt);
        }

        protected override bool ReceiveRecover(object message)
        {
            ActorState state;

            if (message is IEvent)
                UpdateState(message as IEvent);
            else if (message is SnapshotOffer && (state = ((SnapshotOffer)message).Snapshot as ActorState) != null)
                State = state;
            else if (message is RecoveryCompleted)
                Console.WriteLine($"{PersistenceId} Recovery Completed.");
            else
               return false;
            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            base.ReceiveCommand(message);

            if (message is AddLineItemToBasketCommand)
            {
                var cmd = message as AddLineItemToBasketCommand;
                Persist(new AddedLineItemToBasketEvent(cmd.LineItem), UpdateState);                
            }
            else return false;
            return true;
        }
    }
}
