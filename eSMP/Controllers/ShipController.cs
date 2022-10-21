using eSMP.Services.ShipRepo;
using eSMP.VModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipController : ControllerBase
    {
        private readonly IShipReposity _shipReposity;
        public static string TOKEN = "D1f7d11dFA5D417D35F136A02e0C812EEC613Fcb";
        public ShipController(IShipReposity shipReposity)
        {
            _shipReposity = shipReposity;
        }
        [HttpGet]
        public async Task<IActionResult> GetFeeShipAsync(string province, string district, string pick_province, string pick_district, int weight, string deliver_option)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Token", TOKEN);
            var shipResponse = await client.GetAsync(string.Format("https://services.giaohangtietkiem.vn/services/shipment/fee?province={0}&district={1}&pick_province={2}&pick_district={3}&weight={4}&deliver_option={5}", province,district,pick_province,pick_district,weight,deliver_option));
            var jsonreponse = await shipResponse.Content.ReadFromJsonAsync<FeeReponse>();
            //JsonConvert.DeserializeObject<object>(jsonString)
            return Ok(jsonreponse);
        }
        [HttpPost]
        public IActionResult CallBackGHTK(string label_id, string partner_id, int status_id, string action_time, string reason_code, string reason, float weight, int fee, int return_part_package)
        {
            if (_shipReposity.CallBackAsync(label_id, partner_id, status_id, action_time, reason_code, reason))
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpGet]
        [Route("createoder")]
        public IActionResult Createorder(int orderID)
        {
            _shipReposity.CreateOrder(orderID);
            return Ok();
        }
    }
}
