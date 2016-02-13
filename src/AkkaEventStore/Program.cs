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
                BasicUsage(system);

                Console.ReadLine();
            }
        }

        private static void BasicUsage(ActorSystem system)
        {
            var aref = system.ActorOf(Props.Create<BasketActor>(), "basket-actor");
            aref.Tell(new AddLineItemToBasketCommand(new LineItem() { Price = 20, ProductId = "Product-2", Quantity = 3 }));
            aref.Tell("print");
        }
    }
}
