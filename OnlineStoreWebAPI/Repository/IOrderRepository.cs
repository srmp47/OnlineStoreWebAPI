using OnlineStoreWebAPI.Model;
using OnlineStoreWebAPI.Pagination;

namespace OnlineStoreWebAPI.Repository
{
    public interface IOrderRepository
    {
        public Task<IEnumerable<Order>> getAllOrdersAsync(PaginationParameters paginationParameters);
        public Task<IEnumerable<Order>> getAllOrdersOfUserByIdAsync
            (int userId,PaginationParameters paginationParameters);
        // get one order of one user
        public Task<Order?> getOrderByOrderIdAsync( int orderId);
        public Task<bool> isThereOrderByIdAsync(int id);
        public Task<Order> createNewOrderAsync(Order order);
        public Task<Order> updateOrderAsync(Order order);
        public Task cancelOrderByIdAsync(int id);
        public Task setUserInOrder(Order order,int userId);
        public Task setOrderAndProductInOrderItem(OrderItem orderItem);
        public Task<Order> deleteOrderByIdAsync(int id);
        public  Task<IEnumerable<OrderItem>> getAllOrderItemsByOrderIdAsync(int orderId);
        public Task<Order> changeOrderStatusByOrderIdAsync(int id, OrderStatus status);
    }
}
