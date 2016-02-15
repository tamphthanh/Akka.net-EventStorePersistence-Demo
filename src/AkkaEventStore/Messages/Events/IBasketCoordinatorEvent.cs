namespace AkkaEventStore.Messages.Events
{
    public interface IBasketCoordinatorEvent
    {
        int Apply(int counter);
    }
}
