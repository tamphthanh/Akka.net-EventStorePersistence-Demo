namespace AkkaEventStore.Messages.Events
{
    public interface IEvent<T>
    {
        T Apply(T data);
    }
}
