using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using SSBD.API.Models;
using System.IO;
namespace SSBD.API.Data{
    public class SqlRepository : IRepository{
        private readonly ILogger<SqlRepository> logger;
        
        private readonly string connectionstring = Environment.GetEnvironmentVariable("connectionstring");
        private readonly string[] stringvalues = {"Classic Single Burger","Classic Double Burger","Classic Triple Burger","Single Cheese Burger","Double Cheese Burger","Triple Cheese Burger","Veggie Burger","Chicken Burger","Chicken Fried Steak","Fries","Onion Rings","Fried Chicken Pieces","Fried Cheese Sticks","Fried Zucchini","Coca-Cola","Pepsi","Sprite","Dr. Pepper","Lemonade"};
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
        public async Task<MyOrder> InsertOrder(MyOrder order){
            int newid;
            using (SqlConnection connection = new SqlConnection(connectionstring)){
                await connection.OpenAsync();
                string sqlQuery = "INSERT INTO ssburger.CompleteOrder(Name,Complete,Updateon) OUTPUT INSERTED.ID VALUES(@name,@complete,@updateon)";
                using SqlCommand sqlCmd = new SqlCommand(sqlQuery,connection);
                sqlCmd.Parameters.AddWithValue("@name",order.Name);
                sqlCmd.Parameters.AddWithValue("@complete","N");
                sqlCmd.Parameters.AddWithValue("@updateon",DateTime.Now);
                newid = (int) sqlCmd.ExecuteScalar();
                order.Id = newid;
                //string tolog = "we have an id? check check" + newid;
                //logger.LogInformation(tolog);
                connection.Close();
            }
            using (SqlConnection connection = new SqlConnection(connectionstring)){
                await connection.OpenAsync();
                string sqlQuery = "INSERT INTO ssburger.MenuOrder(Order_Id,MenuId,AddOn) VALUES(@orderid,@menuid,@addon)";
                using SqlCommand sqlCmd = new SqlCommand(sqlQuery,connection);
                sqlCmd.Parameters.AddWithValue("orderid",newid);
                sqlCmd.Parameters.AddWithValue("menuid",-1);
                sqlCmd.Parameters.AddWithValue("addon","na");
                for(int i = 0; i < order.Items.Count(); i++){
                    string item = order.Items[i];
                    int index = Array.IndexOf(stringvalues,item);
                    sqlCmd.Parameters[1].Value = index+1;
                    sqlCmd.Parameters[2].Value = order.AddOns[i];
                    await sqlCmd.ExecuteNonQueryAsync();
                    //logger.LogInformation("added value of index? check check" + index);//added value
                }
                connection.Close();
            }
            return order;
        } 
        public async Task <decimal> GetItemPrice(int itemid){
            using SqlConnection connection = new SqlConnection(connectionstring);
            await connection.OpenAsync();
            Console.WriteLine("the incoming id is " + itemid);
            string sqlQuery = "SELECT BasePrice FROM ssburger.MenuItem WHERE Id=@myid";
            using SqlCommand sqlCmd = new(sqlQuery, connection);
            sqlCmd.Parameters.AddWithValue("@myid",itemid);
            using SqlDataReader reader = await sqlCmd.ExecuteReaderAsync();
            decimal price = 0.10M;
            while(reader.Read()){
                price = reader.GetDecimal(0);
                Console.WriteLine("price from db is " + price);
            }
            return price;
        }
        public async Task<string> DeleteOrderByIdAsync(int id){
            throw new NotImplementedException();

        }
        public async Task<string> UpdateOrderByIdAsync(int id, int complete){
            throw new NotImplementedException();

        }
        public async Task<List<MyOrder>> SearchOrders(string? name, string? pending){
            using SqlConnection connection = new SqlConnection(connectionstring);
            await connection.OpenAsync();
            string sqlQuery = "";
            bool searchbyname = true;
            if(name != null && name.Count() > 1){
                sqlQuery = "SELECT Order_Id, CompleteOrder.Name, MenuItem.Name AS Item, AddOn from ssburger.MenuOrder INNER JOIN ssburger.CompleteOrder ON ssburger.MenuOrder.Order_Id=ssburger.CompleteOrder.Id INNER JOIN ssburger.MenuItem ON ssburger.MenuOrder.MenuId = ssburger.MenuItem.Id WHERE ssburger.CompleteOrder.Name=@myname";
            }
            else if(pending.ToUpper() == "Y" || pending.ToUpper() == "N"){
                sqlQuery = "SELECT Order_Id, CompleteOrder.Name, MenuItem.Name AS Item, AddOn from ssburger.MenuOrder INNER JOIN ssburger.CompleteOrder ON ssburger.MenuOrder.Order_Id=ssburger.CompleteOrder.Id INNER JOIN ssburger.MenuItem ON ssburger.MenuOrder.MenuId = ssburger.MenuItem.Id WHERE Complete=@complete";
                searchbyname = false;
            }
            else{
                return null;
            }
            
            using SqlCommand sqlCmd = new(sqlQuery!, connection);
            if(searchbyname){
                sqlCmd.Parameters.AddWithValue("@myname",name);
            }
            else{
                sqlCmd.Parameters.AddWithValue("@complete",pending.ToUpper());
            }
            using SqlDataReader reader = await sqlCmd.ExecuteReaderAsync();
            int myid = 0;
            string outname = null;
            string outcomplete;
            List<string> items = new List<string>();
            List<string> addons = new List<string>();
            List<MyOrder> myorders = new List<MyOrder>();
            while(reader.Read()){
                myid = reader.GetInt32(0);
                name = reader.GetString(1);
                string item = reader.GetString(2);
                string addon = reader.GetString(3);
                items.Add(item);
                addons.Add(addon);
                //MyOrder myOrder = new MyOrder(myid,outname,items.ToArray(),addons.ToArray());
                myorders.Add(new MyOrder(myid,outname,items.ToArray(),addons.ToArray()));
            }
            
            connection.Close();
            return myorders;
        }

        public async Task<Models.MyOrder> CompleteOrder(int id){
            using (SqlConnection connection = new SqlConnection(connectionstring)){
            await connection.OpenAsync();
            string sqlQuery = @"UPDATE ssburger.CompleteOrder SET Complete='Y', Updateon=@dtime WHERE Order_Id=@myid";
            using SqlCommand sqlCmd = new(sqlQuery, connection);
            sqlCmd.Parameters.AddWithValue("@myid",id);
            sqlCmd.Parameters.AddWithValue("@dtime",DateTime.Now);
            sqlCmd.ExecuteNonQuery();
            connection.Close();
            }
            SqlConnection connection1 = new SqlConnection(connectionstring);
            await connection1.OpenAsync();
            string sqlQuery2 = "SELECT Order_Id, CompleteOrder.Name, MenuItem.Name AS Item, AddOn from ssburger.MenuOrder INNER JOIN ssburger.CompleteOrder ON ssburger.MenuOrder.Order_Id=ssburger.CompleteOrder.Id INNER JOIN ssburger.MenuItem ON ssburger.MenuOrder.MenuId = ssburger.MenuItem.Id WHERE Order_Id=@myid";
            using SqlCommand sqlCommand = new(sqlQuery2,connection1);
            using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();
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
            connection1.Close();
            return myOrder;
        }
    }
}
