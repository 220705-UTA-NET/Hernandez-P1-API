using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using SSBD.API.Models;
using SSBD.API.Data;
namespace SSBD.API.Controllers{
    [ApiController]
    [Route("api")]
    public class ProcessRequest : ControllerBase{
        private readonly ILogger<ProcessRequest> logger;
        public ProcessRequest(ILogger<ProcessRequest> logger){
            this.logger = logger;
        }
        [HttpGet("/")]
        public ContentResult GetInfo(){
            string json = @"{
            ""root and info path"":""/api/"",
            ""status"":""queryresult"",
            ""order"":""order"",
            ""total"":""total"",
            ""msg"":""order updated with correct id""}"; 
            var result = new ContentResult(){
                StatusCode = 200,
                ContentType = "application/json",
                Content = json
            };
            return result;
        }
        [HttpGet("helloworld")]
        public ContentResult PrintHello(){
            string json = JsonSerializer.Serialize("{hello:world}");
            var result = new ContentResult(){
                StatusCode = 200,
                ContentType = "application/json",
                Content = json
            };
            return result;
        }
        [HttpPost("submitorder")]
        public async Task<ContentResult> InsertOrder([FromBody] JsonElement data){
            Models.MyOrder order = JsonSerializer.Deserialize<Models.MyOrder>(data);
            decimal total = 0.00M;
            
            var tasks = new List<Task<decimal>>();
            for(int i = 0; i < order.Items.Count(); i++){

                var value = GetItemPrice(order.Items[i], order.AddOns[i]);
                tasks.Add(value);

            }
            decimal[] totalarray = await Task.WhenAll(tasks);
            total = totalarray.Sum();
            logger.LogInformation("I have a total of " + total);
            logger.LogInformation(order.AddOns[0].ToString());
            IRepository repo = new SqlRepository();
            MyOrder queryresult = await repo.InsertOrder(order);
            MyResponse response = new MyResponse(queryresult, total, "order updated with correct id");
            string json = JsonSerializer.Serialize(response);
            var result = new ContentResult(){
                StatusCode = 200,
                ContentType = "application/json",
                Content = json
            };
            Console.WriteLine(json);
            return result;
        }
        [HttpGet("getorder/{Id}")]
        public async Task<ContentResult> GetOrderById(int Id){
            IRepository repo = new SqlRepository();
            MyOrder queryresult = await repo.GetOrderByIdAsync(Id);
            decimal total = 0.00M;
            
            var tasks = new List<Task<decimal>>();
            for(int i = 0; i < queryresult.Items.Count(); i++){

                var value = GetItemPrice(queryresult.Items[i], queryresult.AddOns[i]);
                tasks.Add(value);

            }
            decimal[] totalarray = await Task.WhenAll(tasks);
            total = totalarray.Sum();
            MyResponse response = new MyResponse(queryresult, total,"retrieved");
            System.Console.WriteLine(response.msg);
            //string json = JsonSerializer.Serialize(queryresult);
            string json = JsonSerializer.Serialize(response);
            var result = new ContentResult(){
                StatusCode = 200,
                ContentType = "application/json",
                Content = json
            };
            return result;
        }
        [HttpGet("searchorderbyattributes")]
        public async Task<ContentResult> GetOrderByAttribute(string? name, string? complete){
            IRepository repo = new SqlRepository();
            List<MyOrder> myOrders = await repo.SearchOrders(name,"Y");
            System.Console.WriteLine(myOrders.Count());
            var result = new ContentResult(){
                StatusCode = 200,
                ContentType = "application/json",
                Content = "{'status':'ok'}"
            };
            return result;
        }
        private async Task<Decimal> GetItemPrice(string item, string addon){
            string[] stringvalues = {"Classic Single Burger","Classic Double Burger","Classic Triple Burger","Single Cheese Burger","Double Cheese Burger","Triple Cheese Burger","Veggie Burger","Chicken Burger","Chicken Fried Steak","Fries","Onion Rings","Fried Chicken Pieces","Fried Cheese Sticks","Fried Zucchini","Coca-Cola","Pepsi","Sprite","Dr. Pepper","Lemonade"};
            int index = Array.IndexOf(stringvalues,item);
            Console.WriteLine(index);
            IRepository repo = new SqlRepository();
            Decimal price = await repo.GetItemPrice(index+1);
            addon = addon.ToLower();
            if(addon.Contains("extra")){
                price += (decimal)0.25;
            }
            if(addon.Contains("double")){
                price += (decimal)1.00;
            }
            if(addon.Contains("triple")){
                price += (decimal)2.00;
            }
            
            return price;
        }
    }
    
}
