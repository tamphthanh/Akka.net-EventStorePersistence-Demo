namespace AkkaEventStore.Messages.Events
{
    public class CreateNewBasketEvent : IEvent<int>
    {
        public int Apply(int counter) => counter + 1;        
    }
}
