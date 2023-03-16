using CustomerApi.Data;
using CustomerApi.Models;
using EasyNetQ;
using SharedModels;

namespace CustomerApi.Infrastructure
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
                bus.PubSub.Subscribe<OrderCreatedMessage>("customerApiCreated", HandleOrderCreated);
                bus.PubSub.Subscribe<OrderPaidMessage>("customerApiPaid", HandleOrderPaid);

                lock (this)
                {
                    Monitor.Wait(this);
                }
            }
        }

        private void HandleOrderCreated(OrderCreatedMessage message)
        {
            using var scope = _provider.CreateScope();
            var services = scope.ServiceProvider;
            var customerRepository= services.GetService<IRepository<Customer>>();

            if (message.CustomerId.HasValue)
            {
                var customerId = message.CustomerId.Value;
                if (CustomerHasGoodStanding(customerId, customerRepository))
                {
                    var replyMessage = new OrderAcceptedMessage
                    {
                        OrderId = message.OrderId
                    };

                    bus.PubSub.Publish(replyMessage);
                }
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

        private void HandleOrderPaid(OrderPaidMessage message)
        {
            using var scope = _provider.CreateScope();
            var services = scope.ServiceProvider;
            var customerRepository = services.GetService<IRepository<Customer>>();

            if (CustomerHasGoodStanding(message.CustomerId, customerRepository))
            {
                var customer = customerRepository.Get(message.CustomerId);
                customer.GoodCreditStanding = true;
                customerRepository.Edit(customer);

                var replyMessage = new OrderPaidMessage
                {
                    CustomerId = message.CustomerId,
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

        private static bool CustomerHasGoodStanding(int customerId, IRepository<Customer> customerRepository)
        {
            var customer = customerRepository.Get(customerId);
            if (customer != null && customer.GoodCreditStanding)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
