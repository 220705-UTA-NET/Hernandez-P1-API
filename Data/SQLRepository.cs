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
            //string sqlQuery = "SELECT Id, Name, Complete, Updateon from ssburger.CompleteOrder where Id=@myid";
            //my sql must match this select Order_Id, CompleteOrder.Name, MenuItem.Name AS Item, AddOn from ssburger.MenuOrder INNER JOIN ssburger.CompleteOrder ON ssburger.MenuOrder.Order_Id=ssburger.CompleteOrder.Id JOIN ssburger.MenuItem ON ssburger.MenuOrder.MenuId = ssburger.MenuItem.Id WHERE Order_Id=1;
            string sqlQuery = "SELECT Order_Id, CompleteOrder.Name, MenuItem.Name AS Item, AddOn from ssburger.MenuOrder INNER JOIN ssburger.CompleteOrder ON ssburger.MenuOrder.Order_Id=ssburger.CompleteOrder.Id INNER JOIN ssburger.MenuItem ON ssburger.MenuOrder.MenuId = ssburger.MenuItem.Id WHERE Order_Id=@myid";
            using SqlCommand sqlCmd = new(sqlQuery, connection);
            sqlCmd.Parameters.AddWithValue("@myid",id);
            using SqlDataReader reader = await sqlCmd.ExecuteReaderAsync();
            //MyOrder myOrder = new MyOrder();
            bool havevalue = true;
            //havevalue = !reader.IsDBNull(0);
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
                //DateTime dt = reader.GetDateTime(3);
                //DateTime dt = DateTime.Now;
                //int[] items = {1,2};
                //string[] addons = {"without onion","with onion"};
                //myOrder = new MyOrder(myid,name,items,addons);
                items.Add(item);
                addons.Add(addon);
            }
            MyOrder myOrder = new MyOrder(myid,name,items.ToArray(),addons.ToArray());
            //sqlQuery = "SELECT Id, Name, Complete, Updateon from ssburger.CompleteOrder where Order_Id=@myid";
            //sqlCmd = new(sqlQuery, connection);
            //sqlCmd.Parameters.AddWithValue("@myid",id);
            //reader = await sqlCmd.ExecuteReaderAsync();
            connection.Close();
            //if(!havevalue){
            //    myOrder = new MyOrder(-1,"NA",new int[] {-1},new string[] {"-1"});
            //}
            //MyOrder myOrder = new MyOrder();
            //myOrder.Name = "no name";
            //myOrder.Id = -1;
            //myOrder.Items = new int[] {1,2};
            //myOrder.AddOn = new string[] {"no tomatoes","no salt"};
            
            return myOrder;
        }
/*         public async Task<List<MenuOrder>> GetItemsByOrderId(int id){
            using SqlConnection connection = new SqlConnection(connectionstring);
            await connection.OpenAsync();
            string sqlQuery = "select Order_Id, Name, AddOn from ssburger.MenuOrder INNER JOIN ssburger.MenuItem ON ssburger.MenuOrder.MenuId = ssburger.MenuItem.Id WHERE Order_Id=@orderid;";
            using SqlCommand sqlCmd = new(sqlQuery, connection);
            sqlCmd.Parameters.AddWithValue("@orderid",id);
            using SqlDataReader reader = await sqlCmd.ExecuteReaderAsync();
            //MyOrder myOrder = new MyOrder();
            //bool havevalue = true;
            //havevalue = !reader.IsDBNull(0);
            List<MenuOrder> menuOrders = new List<MenuOrder>();
            while(reader.Read()){
                int myid = reader.GetInt32(0);
                string name = reader.GetString(1);
                string addon = reader.GetString(2);

                //DateTime dt = reader.GetDateTime(3);
                //DateTime dt = DateTime.Now;
                //int[] items = {1,2};
                //string[] addons = {"without onion","with onion"};
                //myOrder = new MyOrder(myid,name,items,addons);
                menuOrders.Add(new MenuOrder(myid,name,addon));
            } 
            //sqlQuery = "SELECT Id, Name, Complete, Updateon from ssburger.CompleteOrder where Order_Id=@myid";
            //sqlCmd = new(sqlQuery, connection);
            //sqlCmd.Parameters.AddWithValue("@myid",id);
            //reader = await sqlCmd.ExecuteReaderAsync();
            connection.Close();
            //if(!havevalue){
            //    myOrder = new MyOrder(-1,"NA",new int[] {-1},new string[] {"-1"});
            //}
            //MyOrder myOrder = new MyOrder();
            //myOrder.Name = "no name";
            //myOrder.Id = -1;
            //myOrder.Items = new int[] {1,2};
            //myOrder.AddOn = new string[] {"no tomatoes","no salt"};
            return menuOrders;
        }*/
        public async Task<string> DeleteOrderByIdAsync(int id){
            string status = "success";
            return status;
        }
        public async Task<string> UpdateOrderByIdAsync(int id, int complete){
            string status = "success";
            return status;
        }
        public async Task<List<MyOrder>> GetPendingOrders(){
            /*
            MyOrder myOrder = new MyOrder();
            myOrder.Name = "no name";
            myOrder.Id = -1;
            myOrder.Items = new int[] {1,2};
            myOrder.AddOn = new string[] {"no tomatoes","no salt"};
            List<Models.MyOrder> someOrders = new List<MyOrder>();
            someOrders.Add(myOrder);
            return someOrders;
            */
            throw new NotImplementedException();
        }
        public async Task<List<MyOrder>> GetCompletedOrders(){
            /*MyOrder myOrder = new MyOrder();
            myOrder.Name = "no name";
            myOrder.Id = -1;
            myOrder.Items = new int[] {1,2};
            myOrder.AddOn = new string[] {"no tomatoes","no salt"};
            List<Models.MyOrder> someOrders = new List<MyOrder>();
            someOrders.Add(myOrder);
            return someOrders;*/
            throw new NotImplementedException();
        }
    }
    //helper classes
    public class CompleteOrder{
        public int Id {get;set;}
        public string Name {get;set;}
        public string Complete {get;set;}
        public DateTime Updateon {get;set;}
    }
    
    public class MenuOrder{
        public int Order_id {get;set;}
        public string AddOn {get;set;}
        public string MenuItem {get;set;}
        public MenuOrder(int id, string menuitem, string addon){
            this.Order_id = id;
            this.MenuItem = menuitem;
            this.AddOn = addon;
        }
    }

}