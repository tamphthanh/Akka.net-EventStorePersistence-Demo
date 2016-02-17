using Akka.Actor;
using AkkaEventStore.Actors;
using System;
using Akka.Persistence.EventStore;
using Akka.Configuration;
using AkkaEventStore.Models;
using System.Threading;
using AkkaEventStore.Messages.Commands;
using AkkaEventStore.Messages;

namespace AkkaEventStore
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            akka {
                actor {
                    serializers {
                        wire = ""Akka.Serialization.WireSerializer, Akka.Serialization.Wire""
                    }
                    serialization-bindings {
                        ""System.Object"" = wire
                    }
                }
            }
            */
            var config = ConfigurationFactory.ParseString(@"

            akka.persistence {
                publish-plugin-commands = on
                journal {
                    plugin = ""akka.persistence.journal.eventstore""
                    eventstore {
                        class = ""Akka.Persistence.EventStore.Journal.EventStoreJournal, Akka.Persistence.EventStore""
                        plugin-dispatcher = ""akka.actor.default-dispatcher""
                        server-host = ""127.0.0.1""
                        server-tcp-port = 4532
                        connection-settings-factory = ""Akka.Persistence.EventStore.DefaultConnectionSettingsFactory, Akka.Persistence.EventStore""
                    }
                }
            }");

            using (var system = ActorSystem.Create("AkkaEventStore", config))
            {
                EventStorePersistence.Init(system);

                Start(system);

                Console.ReadLine();
            }
        }

        private static void Start(ActorSystem system)
        {
            Console.WriteLine("System Started...");            
            var aref = system.ActorOf(Props.Create<BasketCoordinatorActor>(), "basket-coordinator");
            
            var counter = 0;
            var total = 1;
            for (int i = 0; i < total; i++)
            {
                aref.Tell(new CreateNewBasketCommand());
            }

            while (true)
            {
                //load testing
                /*
                if (counter % 50 == 0)
                {
                    Thread.Sleep(1);
                    Console.WriteLine(counter);
                }
                counter++;
                var tokens = new[] { "put", "basket-" + new Random().Next(0, total) , "p1", "20", "10" }; // using for load testing
                */

                var command = Console.ReadLine();                
                var tokens = command.Split(' ');

                switch (tokens[0])
                {
                    case "peek":
                        if (tokens.Length > 1)
                        {
                            aref.Tell("peekBasket " + tokens[1]);
                        }
                        else
                        {
                            aref.Tell("peek");
                        }
                        break;
                    case "create":
                        aref.Tell(new CreateNewBasketCommand());
                        break;
                    case "put":
                        if (tokens.Length == 5)
                        {
                            aref.Tell(new AddLineItemToBasketMessage(
                                tokens[1]
                                , new LineItem()
                                {
                                    ProductId = tokens[2]
                                    ,Quantity = Convert.ToInt32(tokens[3])
                                    ,Price = Convert.ToInt32(tokens[4])
                                }));
                        }
                        else
                        {
                            Console.WriteLine("Invalid parameters");
                        }
                        break;
                    case "remove":
                        if (tokens.Length == 3)
                        {
                            aref.Tell(new RemoveLineItemFromBasketMessage(
                                tokens[1]
                                , new LineItem()
                                {
                                    Id = tokens[2]
                                }));
                        }
                        else
                        {
                            Console.WriteLine("Invalid parameters");
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
