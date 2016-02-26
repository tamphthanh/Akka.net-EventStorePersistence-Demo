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
        int counter = 0;

        public BasketCoordinatorActor()
        {
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

            //var connection = EventStoreConnection.Create(
            //    ConnectionSettings.Create().KeepReconnecting(),
            //    ClusterSettings.Create().DiscoverClusterViaGossipSeeds()
            //        .SetGossipTimeout(TimeSpan.FromMilliseconds(500))
            //        .SetGossipSeedEndPoints(new IPEndPoint[] {
            //            new IPEndPoint(IPAddress.Loopback, 1114),
            //            new IPEndPoint(IPAddress.Loopback, 2114),
            //            new IPEndPoint(IPAddress.Loopback, 3114),
            //    }));

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

            Console.WriteLine($"[{DateTime.Now}] Basket Coordinator Recovered.");

            Receive<CreateNewBasketMessage>(message =>
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
