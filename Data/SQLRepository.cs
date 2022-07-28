using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using SSBD.Models;
using System.IO;
namespace SSBD.Data{
    public class SqlRepository : IRepository{
        private readonly ILogger<SqlRepository> logger;
        
        private readonly string connectionstring = Environment.GetEnvironmentVariable("connectionstring");
        //public SqlRepository(string connectionstring, ILogger<SqlRepository> logger){
        //    this.logger = logger;
        //}
        public async Task<MyOrder> GetOrderByIdAsync(int id){
            using SqlConnection connection = new SqlConnection(connectionstring);
            await connection.OpenAsync();
            string sqlQuery = "SELECT Order_Id, CompleteOrder.Name, MenuItem.Name AS Item, AddOn from ssburger.MenuOrder INNER JOIN ssburger.CompleteOrder ON ssburger.MenuOrder.Order_Id=ssburger.CompleteOrder.Id INNER JOIN ssburger.MenuItem ON ssburger.MenuOrder.MenuId = ssburger.MenuItem.Id WHERE Order_Id=@myid";
            using SqlCommand sqlCmd = new(sqlQuery, connection);
            sqlCmd.Parameters.AddWithValue("@myid",id);
            using SqlDataReader reader = await sqlCmd.ExecuteReaderAsync();
            int myid = 0;
            string name = null;
            string complete;
            List<string> items = new List<string>();
            List<string> addons = new List<string>();
            while(reader.Read()){
                myid = reader.GetInt32(0);
                name = reader.GetString(1);
                string item = reader.GetString(2);
                string addon = reader.GetString(3);
                items.Add(item);
                addons.Add(addon);
            }
            MyOrder myOrder = new MyOrder(myid,name,items.ToArray(),addons.ToArray());
            connection.Close();
            
            return myOrder;
        }
        public async Task<string> DeleteOrderByIdAsync(int id){
            string status = "success";
            return status;
        }
        public async Task<string> UpdateOrderByIdAsync(int id, int complete){
            string status = "success";
            return status;
        }
        public async Task<List<MyOrder>> GetPendingOrders(){
            
            throw new NotImplementedException();
        }
        public async Task<List<MyOrder>> GetCompletedOrders(){
            
            throw new NotImplementedException();
        }
    }
}