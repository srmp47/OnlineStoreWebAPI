using OnlineStoreWebAPI.Model;

namespace OnlineStoreWebAPI.Repository
{
    public interface IOrderItemRepository
    {
        public Task<IEnumerable<OrderItem>> getAllOrderItemsByOrderIdAsync(int orderId);
        public Task<OrderItem?> getOrderItemByOrderIdAndOrderItemId(int orderId,int orderItemId);
        public Task<bool> isThereOrderItemById(int id);
    }
}
