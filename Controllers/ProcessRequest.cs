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
        //public async Task<ActionResult<SOMETHING>> GetSomething()
        [HttpPost("/SendOrder")]
        public ContentResult GetOrder([FromBody] JsonElement data){
            string json = JsonSerializer.Serialize("msg:success");
            var result = new ContentResult(){
                StatusCode = 200,
                ContentType = "application/json",
                Content = json
            };
            //string otherdata = JsonContent.Create(data).ToString();// .Deserialize<string>(data);
            //otherdata = JsonSerializer.Deserialize<string>((Stream)data);
            /*JsonSerializerOptions options = new JsonSerializerOptions{
                Converters = {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };*/ //unable to use the string enum converter
            Models.MyOrder order = JsonSerializer.Deserialize<Models.MyOrder>(data);
            order.Id = 10; 
            //foreach(var each in order.Items){
                //send data to database
            //}
            //logger.LogInformation(data.ToString());
            logger.LogInformation(order.Items[0].ToString());
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