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

    public class BasketActorState
    {
        public Basket basket = new Basket();

        public BasketActorState Update(IEvent evt)
        {
            return new BasketActorState { basket = evt.Apply(basket) };
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(basket, Formatting.Indented);
        }
    }

    public class BasketActor : PersistentActor
    {
        public override string PersistenceId { get; }

        public BasketActorState State { get; set; }

        // create a new basket
        public BasketActor()
        {
            var id = "basket-13"; // hardcoded for now
            State = new BasketActorState();
            State.basket.Id = id;
            PersistenceId = id;            
            Persist(new CreatedBasketEvent(id), UpdateState);
        }

        public BasketActor(string id)
        {
            State = new BasketActorState();
            State.basket.Id = id;
            PersistenceId = id;
        }

        public void UpdateState(IEvent evt)
        {
            State = State.Update(evt);
        }

        protected override bool ReceiveRecover(object message)
        {
            BasketActorState state;

            if (message is IEvent)
                UpdateState(message as IEvent);
            else if (message is SnapshotOffer && (state = ((SnapshotOffer)message).Snapshot as BasketActorState) != null)
                State = state;
            else return false;
            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            if (message is AddLineItemToBasketCommand)
            {
                var cmd = message as AddLineItemToBasketCommand;
                Persist(new AddedLineItemToBasketEvent(cmd.LineItem), UpdateState);
                return true;
            }
            else if (message as string == "print")
                Console.WriteLine(State);
            else return false;
            return true;
        }
    }
}
