using SSBD.API.Models;
namespace SSBD.API.Data{
    public interface IRepository{
        Task<MyOrder> GetOrderByIdAsync(int id);
        Task<string> InsertOrder(MyOrder order);
        Task<string> DeleteOrderByIdAsync(int id);
        Task<string> UpdateOrderByIdAsync(int id, int complete);
        Task<List<MyOrder>> GetPendingOrders();
        Task<List<MyOrder>> GetCompletedOrders();
        Task<Decimal> GetItemPrice(int itemid);
    }
}