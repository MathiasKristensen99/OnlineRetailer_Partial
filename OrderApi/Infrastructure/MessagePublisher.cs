using EasyNetQ;
using SharedModels;

namespace OrderApi.Infrastructure
{
    public class MessagePublisher : IMessagePublisher, IDisposable
    {
        IBus _bus;

        public MessagePublisher(string connectionString)
        {
            _bus = RabbitHutch.CreateBus(connectionString);
        }

        public void Dispose()
        {
            _bus.Dispose();
        }

        public void PublishOrderCreatedMessage(int? customerId, int orderId, IList<OrderLine> orderLines)
        {
            var message = new OrderCreatedMessage
            {
                CustomerId = customerId,
                OrderId = orderId,
                OrderLines = orderLines
            };

            _bus.PubSub.Publish(message);
        }
        public void PublishOrderShippedMessage(int orderId)
        {
            var message = new OrderShippedMessage
            {
                OrderId = orderId
            };

            _bus.PubSub.Publish(message);
        }
        public void PublishOrderCancelledMessage(int orderId)
        {
            var message = new OrderCancelledMessage
            {
                OrderId = orderId
            };

            _bus.PubSub.Publish(message);
        }
        public void PublishOrderPaidMessage(int orderId)
        {
            var message = new OrderPaidMessage
            {
                OrderId = orderId
            };

            _bus.PubSub.Publish(message);
        }
        public void PublishOrderAcceptedMessage(int orderId)
        {
            var message = new OrderAcceptedMessage
            {
                OrderId = orderId
            };

            _bus.PubSub.Publish(message);
        }
        public void PublishOrderRejectedMessage(int orderId)
        {
            var message = new OrderRejectedMessage
            {
                OrderId = orderId
            };

            _bus.PubSub.Publish(message);
        }
    }
}
