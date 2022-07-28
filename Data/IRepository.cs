using SSBD.Models;
namespace SSBD.Data{
    public interface IRepository{
        Task<MyOrder> GetOrderByIdAsync(int id);
        Task<string> DeleteOrderByIdAsync(int id);
        Task<string> UpdateOrderByIdAsync(int id, int complete);
        Task<List<MyOrder>> GetPendingOrders();
        Task<List<MyOrder>> GetCompletedOrders();
    }
}