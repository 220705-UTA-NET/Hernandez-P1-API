using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using SSBD.Models;
using SSBD.Data;
namespace SSBD.Controllers{
    [ApiController]
    [Route("[controller]")]
    public class ProcessRequest : ControllerBase{
        private readonly ILogger<ProcessRequest> logger;
        public ProcessRequest(ILogger<ProcessRequest> logger){
            this.logger = logger;
        }
        [HttpGet("helloworld")]
        public ContentResult PrintHello(){
            string json = JsonSerializer.Serialize("hello:world");
            var result = new ContentResult(){
                StatusCode = 200,
                ContentType = "application/json",
                Content = json
            };
            return result;
        }
        [HttpPost("SubmitOrder")]
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
            //decimal priceofburger = await GetItemPrice("Classic Single Burger","with extra cheese");
            //logger.LogInformation("price of the burger with extra cheese should be " + priceofburger);
            logger.LogInformation("I have a total of " + total);
            logger.LogInformation(order.AddOns[0].ToString());
            //IRepository repo = new SqlRepository();
            //string queryresult = await repo.InsertOrder(order);
            string json = JsonSerializer.Serialize("msg:" + "queryresult" + ",total:"+total); 
            var result = new ContentResult(){
                StatusCode = 200,
                ContentType = "application/json",
                Content = json
            };
            return result;
        }
        [HttpGet("getorder/{id}")]
        public async Task<ContentResult> GetOrderById(int Id){
            IRepository repo = new SqlRepository();
            MyOrder queryresult = await repo.GetOrderByIdAsync(Id);
            string json = JsonSerializer.Serialize(queryresult);
            var result = new ContentResult(){
                StatusCode = 200,
                ContentType = "application/json",
                Content = json
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
            if(addon.Contains("double")){
                price += (decimal)2.00;
            }
            
            return price;
        }
    }
    
}