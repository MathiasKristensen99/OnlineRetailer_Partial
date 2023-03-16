using SharedModels;

namespace OrderApi.Infrastructure
{
    public interface IMessagePublisher
    {
        void PublishOrderCreatedMessage(int customerId, int orderId, IList<OrderLine> orderLines);
        void PublishOrderAcceptedMessage(int orderId);
        void PublishOrderCancelledMessage(int orderId);
        void PublishOrderPaidMessage(int orderId, int customerId);
        void PublishOrderRejectedMessage(int orderId);
        void PublishOrderShippedMessage(int orderId);
    }
}
