using EasyNetQ;
using ProductApi.Data;
using ProductApi.Models;
using SharedModels;

namespace ProductApi.Infrastructure
{
    public class MessageListener
    {
        private IServiceProvider _provider;
        private string connectionString;
        private IBus bus;

        public MessageListener(IServiceProvider provider, string connectionString)
        {
            _provider = provider;
            this.connectionString = connectionString;
        }

        public void Start()
        {
            using (bus = RabbitHutch.CreateBus(connectionString))
            {
                bus.PubSub.Subscribe<OrderCreatedMessage>("productApiCreated", HandleOrderCreated);
            }

            lock (this)
            {
                Monitor.Wait(this);
            }
        }

        private void HandleOrderCreated(OrderCreatedMessage message)
        {
            using var scope = _provider.CreateScope();
            var services = scope.ServiceProvider;
            var productRepos = services.GetService<IRepository<Product>>();

            if (ProductItemsAvailable(message.OrderLines, productRepos))
            {
                foreach (var orderLine in message.OrderLines)
                {
                    var product = productRepos.Get(orderLine.ProductId);
                    product.ItemsReserved += orderLine.Quantity;
                    productRepos.Edit(product);
                }

                var replyMessage = new OrderAcceptedMessage
                {
                    OrderId = message.OrderId
                };

                bus.PubSub.Publish(replyMessage);
            }
            else
            {
                var replyMessage = new OrderRejectedMessage
                {
                    OrderId = message.OrderId
                };

                bus.PubSub.Publish(replyMessage);
            }
        }

        private bool ProductItemsAvailable(IList<OrderLine> orderLines, IRepository<Product> productRepos)
        {
            foreach (var orderLine in orderLines)
            {
                var product = productRepos.Get(orderLine.ProductId);
                if (orderLine.Quantity > product.ItemsInStock - product.ItemsReserved)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
