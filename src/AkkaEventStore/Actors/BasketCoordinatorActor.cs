using Akka.Actor;
using AkkaEventStore.Actors.Messages.Commands;
using AkkaEventStore.Messages;
using AkkaEventStore.Messages.Commands;
using EventStore.ClientAPI;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AkkaEventStore.Actors
{
    public class BasketCoordinatorActor : ReceiveActor
    {
        private IDictionary<string, IActorRef> baskets = new Dictionary<string, IActorRef>();
        int counter = 1;

        public BasketCoordinatorActor()
        {
            /* Projection faking a persisted actor -
fromCategory('basket')
  .when({
    $init : function() {
         return {
            count: 0
        }
    },
    "AkkaEventStore.Messages.Events.CreatedBasketEvent": function(s, e) {
        var count = s.count++;
        emit("baskets", "AkkaEventStore.Messages.Events.CreateNewBasketEvent", {
            "$id": 1,
            "$type": "Akka.Persistence.Persistent, Akka.Persistence",
            "Payload": {
                "$id": "2",
                "$type": "AkkaEventStore.Messages.Events.CreateNewBasketEvent, AkkaEventStore",
            },
            "Sender": {
                "$id": "3",
                "$type": "Akka.Actor.ActorRefBase+Surrogate, Akka",
                "Path": "akka://AkkaEventStore/deadLetters"
            },
            PersistenceId: "baskets",
            "SequenceNr": count,
            "Manifest": ""
        })
    }
  })
            */

            /*

            fromCategory('basket')
              .when({
                $init : function() {
                     return {
                        count: 1
                    }
                },
                "AkkaEventStore.Messages.Events.CreatedBasketEvent": function(s, e) {
                    var count = s.count++;
                    emit("basketsCounter", "Increment", count)
                }
              })
                        */

            // initialize directly from database
            var connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
            connection.ConnectAsync().Wait();
            var streamEvents =
                connection.ReadStreamEventsBackwardAsync("basketsCounter", StreamPosition.End, 1, false).Result;
            if (streamEvents.Events.Length > 0)
            {
                var number = Convert.ToInt32(Encoding.UTF8.GetString(streamEvents.Events[0].Event.Data));
                for (int i = number; i > 0; i--)
                {
                    var basketId = "basket-" + i;                                        
                    baskets.Add(basketId, Context.ActorOf(Props.Create<BasketActor>(basketId), basketId));
                }
                counter = number;
            }

            Receive<CreateNewBasketCommand>(message =>
            {
                var basketId = "basket-" + ++counter;
                baskets.Add(basketId, Context.ActorOf(Props.Create<BasketActor>(basketId), basketId));
                baskets[basketId].Tell(new CreateBasketCommand(basketId));
            });

            Receive<AddLineItemToBasketMessage>(message =>
            {
                if (baskets.ContainsKey(message.BasketId))
                    baskets[message.BasketId].Forward(new AddLineItemToBasketCommand(message.LineItem));
                else
                    Console.WriteLine("No such basket");
            });

            Receive<RemoveLineItemFromBasketMessage>(message =>
            {
                if (baskets.ContainsKey(message.BasketId))
                    baskets[message.BasketId].Forward(new RemoveLineItemFromBasketCommand(message.LineItem));
                else
                    Console.WriteLine("No such basket");
            });

            Receive<string>(message =>
            {
                if ((message as string).StartsWith("peekBasket "))
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
            });
        }
    }
}
