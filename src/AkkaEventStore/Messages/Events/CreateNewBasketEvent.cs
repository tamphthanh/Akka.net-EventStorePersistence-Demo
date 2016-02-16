namespace AkkaEventStore.Messages.Events
{
    public class CreateNewBasketEvent : IEvent<int>
    {
        public int Apply(int counter)
        {
            return counter + 1;
        }
    }
}
