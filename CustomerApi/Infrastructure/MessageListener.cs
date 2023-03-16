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

                // Block the thread so that it will not exit and stop subscribing.
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

            if (CustomerHasGoodStanding(message.CustomerId, customerRepository))
            {
                var customer = customerRepository.Get(message.CustomerId);
                customer.GoodCreditStanding = false;
                customerRepository.Edit(customer);

                var replyMessage = new OrderAcceptedMessage()
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

        private void HandleOrderPaid(OrderPaidMessage message)
        {
            using var scope = _provider.CreateScope();
            var services = scope.ServiceProvider;
            var customerRepository = services.GetService<IRepository<Customer>>();

            if (!CustomerHasGoodStanding(message.CustomerId, customerRepository))
            {
                var customer = customerRepository.Get(message.CustomerId);
                customer.GoodCreditStanding = true;
                customerRepository.Edit(customer);

                var replyMessage = new OrderPayAcceptedMessage
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

        private bool CustomerHasGoodStanding(int customerId, IRepository<Customer> customerRepository)
        {
            var customer = customerRepository.Get(customerId);
            return customer is { GoodCreditStanding: true };
        }
    }
}
