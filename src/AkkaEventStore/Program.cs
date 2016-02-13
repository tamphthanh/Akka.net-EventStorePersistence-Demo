using Akka.Actor;
using AkkaEventStore.Actors;
using System;
using Akka.Persistence.EventStore;
using Akka.Configuration;
using AkkaEventStore.Models;

namespace AkkaEventStore
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(@"
            actor {
                serializers {
                    json = ""Akka.Serialization.NewtonSoftJsonSerializer""
                }
            }
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
            //var tokens = new[] { "put", "basket-1", "p1", "20", "10" }; // using for load testing
            while (true)
            {
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
                        aref.Tell(new IncrementBasketIdCommand());
                        break;
                    case "put":
                        if (tokens.Length == 5)
                        {
                            aref.Tell(new AddLineItemToSpecificBasketCommand(
                                tokens[1]
                                , new LineItem()
                                {
                                    ProductId = tokens[2]
                                    ,
                                    Quantity = Convert.ToInt32(tokens[3])
                                    ,
                                    Price = Convert.ToInt32(tokens[4])
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
