using SSBD.API.Models;
namespace SSBD.API.Data{
    public interface IRepository{
        Task<MyOrder> GetOrderByIdAsync(int id);
        Task<MyOrder> InsertOrder(MyOrder order);
        Task<string> DeleteOrderByIdAsync(int id);
        Task<string> UpdateOrderByIdAsync(int id, int complete);
        Task<List<MyOrder>> SearchOrders(string name, string pending);
        Task<Decimal> GetItemPrice(int itemid);
    }
}