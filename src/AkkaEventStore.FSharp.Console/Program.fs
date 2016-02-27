open Akka.Actor;
open AkkaEventStore.Actors;
open System;
open Akka.Persistence.EventStore;
open Akka.Configuration;
open AkkaEventStore.Models;
open AkkaEventStore.Messages;

[<EntryPoint>]
let main argv = 
    let Start(system:ActorSystem)  : Unit =
        let loadTest = true

        Console.WriteLine("System Started...")
        let aref = system.ActorOf(Props.Create<BasketCoordinatorActor>(), "basket-coordinator")
        let total = 10000        
        //[|0..total|] |> Array.iter (aref.Tell(new CreateNewBasketMessage()))   
                      
        while true do                        
            let rn = (new Random()).Next(1, total).ToString()

            let command = 
                match loadTest with                    
                    | false -> Console.ReadLine()
                    | true -> ""

            let tokens = 
                match loadTest with
                    | true -> [|"put"; "basket-" + rn; "p1"; "20"; "10"|]
                    | false -> command.Split(' ')
            
            match tokens.[0] with
                | "peek" when tokens.Length > 1 -> aref.Tell("peekBasket " + tokens.[1])
                | "peek" -> aref.Tell("peek")
                | "create" -> aref.Tell(new CreateNewBasketMessage())
                | "put" when tokens.Length = 5 -> 
                    aref.Tell(new AddLineItemToBasketMessage(
                                tokens.[1]
                                , new LineItem(
                                    ProductId = tokens.[2]
                                    ,Quantity = Convert.ToInt32(tokens.[3])
                                    ,Price = Convert.ToInt32(tokens.[4]))))
                | "remove" when tokens.Length = 3 -> 
                    aref.Tell(new RemoveLineItemFromBasketMessage(
                                tokens.[1], new LineItem(Id = tokens.[2])))
                | _ -> printfn "invalid command"

    let config = ConfigurationFactory.ParseString(@"
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
            akka.persistence {
                publish-plugin-commands = on
                journal {
                    plugin = ""akka.persistence.journal.eventstore""
                    eventstore {
                        class = ""Akka.Persistence.EventStore.Journal.EventStoreJournal, Akka.Persistence.EventStore""
                        plugin-dispatcher = ""akka.actor.default-dispatcher""
                        host=""127.0.0.1""
                        tcp-port = ""1113""
                    }
                }
            }")
    
    (use system = ActorSystem.Create("AkkaEventStore", config)
    EventStorePersistence.Init(system);
    Start(system);
    Console.ReadLine() |> ignore)
    0 // return an integer exit code
