using MassTransit;
using ServiceDefaults.Messaging.Events;

namespace Basket.EventHandlers;

public class ProductPriceChangedEventHandler(BasketService service) : IConsumer<ProductPriceChangedEvent>
{
    public async Task Consume(ConsumeContext<ProductPriceChangedEvent> context)
    {
        await service.UpdateBasketItemProductPrices(context.Message.ProductId, context.Message.Price);
    }
}