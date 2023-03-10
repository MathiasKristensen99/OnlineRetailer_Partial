﻿using EasyNetQ;
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
    }
}