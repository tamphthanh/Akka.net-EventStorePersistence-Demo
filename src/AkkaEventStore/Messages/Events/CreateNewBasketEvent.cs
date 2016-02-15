namespace AkkaEventStore.Messages.Events
{
    public class CreateNewBasketEvent : IBasketCoordinatorEvent
    {
        public int Apply(int counter)
        {
            return counter + 1;
        }
    }
}
