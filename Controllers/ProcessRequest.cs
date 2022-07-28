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
        [HttpGet("/helloworld")]
        public ContentResult PrintHello(){
            string json = JsonSerializer.Serialize("hello:world");
            var result = new ContentResult(){
                StatusCode = 200,
                ContentType = "application/json",
                Content = json
            };
            return result;
        }
        [HttpPost("/SubmitOrder")]
        public async Task<ContentResult> InsertOrder([FromBody] JsonElement data){
            Models.MyOrder order = JsonSerializer.Deserialize<Models.MyOrder>(data);
            logger.LogInformation(order.AddOns[1].ToString());
            IRepository repo = new SqlRepository();
            string queryresult = await repo.InsertOrder(order);
            string json = JsonSerializer.Serialize("msg:" + queryresult);
            var result = new ContentResult(){
                StatusCode = 200,
                ContentType = "application/json",
                Content = json
            };
            return result;
        }
        [HttpGet("/getorder")]
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
    }
    
}