using eSMP.VModels;

namespace eSMP.Services.ShipRepo
{
    //https://khachhang-staging.ghtklab.com
    public class ShipRepository : IShipReposity
    {
        public static string TOKEN = "D1f7d11dFA5D417D35F136A02e0C812EEC613Fcb";
        public static string deliver_option = "none";
        public async Task<Fee> GetFeeAsync(string province, string district, string pick_province, string pick_district, int weight, string deliver_option)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Token", TOKEN);
            HttpResponseMessage shipResponse = await client.GetAsync(string.Format("https://services.giaohangtietkiem.vn/services/shipment/fee?province={0}&district={1}&pick_province={2}&pick_district={3}&weight={4}&deliver_option={5}", province, district, pick_province, pick_district, weight, deliver_option));
            var jsonreponse = await shipResponse.Content.ReadFromJsonAsync<FeeReponse>();
            return jsonreponse.fee;
        }

        Fee IShipReposity.GetFeeAsync(string province, string district, string pick_province, string pick_district, int weight)
        {
            return GetFeeAsync(province, district, pick_province, pick_district, weight, deliver_option).Result;
        }
    }
}
