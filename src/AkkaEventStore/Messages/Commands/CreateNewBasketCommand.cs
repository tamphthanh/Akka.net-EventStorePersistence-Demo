﻿using System;
using AkkaEventStore.Actors;

namespace AkkaEventStore.Messages.Commands
{
    public class CreateNewBasketCommand : ICommand
    {
        public bool Execute(IActorState state)
        {
            throw new NotImplementedException();
        }
    }
}