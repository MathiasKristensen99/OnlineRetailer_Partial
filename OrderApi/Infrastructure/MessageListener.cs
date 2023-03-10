using EasyNetQ;
using OrderApi.Data;
using SharedModels;

namespace OrderApi.Infrastructure
{
    public class MessageListener
    {
        private IServiceProvider provider;
        private string connectionString;
        private IBus bus;

        public MessageListener(IServiceProvider provider, string connectionString)
        {
            this.provider = provider;
            this.connectionString = connectionString;
        }

        public void Start()
        {
            using (bus = RabbitHutch.CreateBus(connectionString))
            {
                bus.PubSub.Subscribe<OrderAcceptedMessage>("orderApiHkAccepted", HandleOrderAccepted);

                bus.PubSub.Subscribe<OrderRejectedMessage>("orderApiHkRejected", HandleOrderRejected);

                lock (this)
                {
                    Monitor.Wait(this);
                }
            }
        }

        private void HandleOrderAccepted(OrderAcceptedMessage message)
        {
            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var orderRepos = services.GetService<IRepository<Order>>();

                var order = orderRepos.Get(message.OrderId);
                order.Status = Order.OrderStatus.completed;
                orderRepos.Edit(order);
            }
        }

        private void HandleOrderRejected(OrderRejectedMessage message)
        {
            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var orderRepos = services.GetService<IRepository<Order>>();

                orderRepos.Remove(message.OrderId);
            }
        }
    }
}
