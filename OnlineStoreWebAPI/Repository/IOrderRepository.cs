using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;

namespace OnlineStoreWebAPI.Repository
{
    public interface IOrderRepository
    {
        public Task<IEnumerable<Order>> getAllOrdersAsync(PaginationParameters paginationParameters);
        public Task<Order?> getOrderByIdAsync(int id);
        public  Task<IEnumerable<Order>> getAllOrdersOfUserByIdAsync(int userId, PaginationParameters paginationParameters);
        public  Task<bool> isThereOrderWithIdAsync(int orderId);
        public Task createNewOrderAsync(Order order);
        public Task<bool> deleteOrderWithIdAsync(int orderId);
        public Task updateOrder(Order order);
        public Task setUserInOrder(Order order, int userId);
        public  Task setOrderAndProductInOrderItem(OrderItem orderItem);
        public Task<int> getUserIdOfOrder(int orderId);
    }
}
