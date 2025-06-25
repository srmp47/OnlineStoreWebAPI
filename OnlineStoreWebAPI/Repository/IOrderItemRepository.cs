using OnlineStoreWebAPI.Model;

namespace OnlineStoreWebAPI.Repository
{
    public interface IOrderItemRepository
    {
        public Task<IEnumerable<OrderItem>> getAllOrderItemsByOrderIdAsync(int orderId);
        public Task<OrderItem?> getOrderItemByOrderIdAndOrderItemId(int orderId,int orderItemId);
        public Task<bool> isThereOrderItemById(int id);
        public Task<OrderItem> createNewOrderItemAsync(OrderItem orderItem);
        public Task<OrderItem> updateOrderItemAsync(OrderItem orderItem);
        public Task<OrderItem> deleteOrderItemByIdAsync(int id);

    }
}
