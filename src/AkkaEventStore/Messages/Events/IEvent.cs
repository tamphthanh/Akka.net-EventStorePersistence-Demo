using AkkaEventStore.Models;

namespace AkkaEventStore.Messages.Events
{
    public interface IEvent
    {
        Basket Apply(Basket basket);
    }
}
