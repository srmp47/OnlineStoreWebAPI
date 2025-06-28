using OnlineStoreWebAPI.Model;

namespace OnlineStoreWebAPI.Repository
{
    public interface IOrderRepository
    {
        public Task<IEnumerable<Order>> getAllOrdersAsync();
        public Task<IEnumerable<Order>> getAllOrdersOfUserByIdAsync(int userId);
        // get one order of one user
        public Task<Order?> getOrderByOrderIdAsync( int orderId);
        public Task<bool> isThereOrderByIdAsync(int id);
        public Task<Order> createNewOrderAsync(Order order);
        public Task<Order> updateOrderAsync(Order order);
        public Task cancelOrderByIdAsync(int id);


    }
}
