using AkkaEventStore.Messages.Events;
using System;
using System.Linq;

namespace AkkaEventStore.Messages.Handlers.Basket
{
    public static class BasketEventHandlers
    {
        public static Models.Basket Handle(Models.Basket basket, IEvent evt)
        {
            if (evt is CreatedBasketEvent)
            {
                return basket;
            }
            else if (evt is AddedLineItemToBasketEvent)
            {
                var newBasket = basket;
                newBasket.LineItems.Add((evt as AddedLineItemToBasketEvent).LineItem);
                return newBasket;
            }
            else if (evt is RemovedLineItemFromBasketEvent)
            {
                var newBasket = basket;
                basket.LineItems = basket.LineItems.Where(
                    li =>
                        li.Id != (evt as RemovedLineItemFromBasketEvent).LineItem.Id)
                    .ToList();
                return newBasket;
            }
            throw new NotImplementedException($"Unable to handle {evt}");
        }
    }
}
