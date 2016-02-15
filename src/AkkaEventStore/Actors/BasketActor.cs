using Akka.Persistence;
using AkkaEventStore.Models;
using Newtonsoft.Json;
using System;

namespace AkkaEventStore.Actors
{
    #region Commands
    public class CreateBasketCommand : ICommand
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

    public class AddLineItemToBasketCommand : ICommand
    {
        public LineItem LineItem { get; private set; }

        public AddLineItemToBasketCommand(LineItem lineItem)
        {
            LineItem = lineItem;
        }

        // execute our command based on current state
        public bool Execute(IActorState state)
        {
            // Validate and do side effects
            if ((state as BasketActorState).basket.LineItems.Count > 100) return false;

            // success
            return true;
        }
    }

    public interface ICommand
    {
        bool Execute(IActorState state);
    }
    #endregion

    #region Events
    public interface IEvent
    {
        Basket Apply(Basket basket);
    }

    public class CreatedBasketEvent : IEvent
    {
        public Basket Basket { get; private set; }

        public CreatedBasketEvent(Basket basket)
        {
            Basket = basket;
        }

        public override string ToString()
        {
            return Basket.Id;
        }

        public Basket Apply(Basket basket)
        {
            return basket;
        }
    }

    public class AddedLineItemToBasketEvent : IEvent
    {
        private Basket _basket { get; set; }
        public LineItem LineItem { get; private set; }

        public AddedLineItemToBasketEvent(LineItem lineItem)
        {
            LineItem = lineItem;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(LineItem);
        }

        public Basket Apply(Basket basket)
        {
            _basket = basket;
            _basket.LineItems.Add(LineItem);
            return _basket;
        }
    }
    #endregion

    public class BasketActorState : IActorState
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

    public class BasketActor : PersistentStateActor
    {
        public override string PersistenceId { get; }

        public override IActorState State { get; set; }

        public BasketActor(string id)
        {
            State = new BasketActorState();
            (State as BasketActorState).basket.Id = id;
            PersistenceId = id;
        }

        public void UpdateState(IEvent evt)
        {
            State = (State as BasketActorState).Update(evt);
        }

        protected override bool ReceiveRecover(object message)
        {
            BasketActorState state;

            if (message is IEvent)
                UpdateState(message as IEvent);
            else if (message is SnapshotOffer && (state = ((SnapshotOffer)message).Snapshot as BasketActorState) != null)
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
                if (cmd.Execute(State)) // check validation and execute side effects
                    Persist(new AddedLineItemToBasketEvent(cmd.LineItem), UpdateState);
                else return false;
            }
            else if (message is CreateBasketCommand)
            {
                var cmd = message as CreateBasketCommand;
                if (cmd.Execute(State))
                {
                    Persist(new CreatedBasketEvent(cmd.basket), UpdateState);
                    Sender.Tell(true, Self);
                }
                else return false;
            }
            else return false;
            return true;
        }
    }
}
