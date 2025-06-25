using OnlineStoreWebAPI.Model;

namespace OnlineStoreWebAPI.Repository
{
    public interface IOrderRepository
    {
        public Task<IEnumerable<Order>> getAllOrdersAsync();
        public Task<IEnumerable<Order>> getAllOrdersOfUserByIdAsync(int userId);
        // get one order of one user
        public Task<Order?> getOrderByUserIdAndOrderIdAsync(int userId, int orderId);
        public Task<bool> isThereOrderById(int id);


    }
}
