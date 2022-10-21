namespace eSMP.VModels
{
    public class ShipModel
    {
    }
    public class FeeShipModel
    {
        public string pick_province { get; set; }
        public string pick_district { get; set; }
        public string province { get; set; }
        public string district { get; set; }
        public int weight { get; set; }
        public string deliver_option { get; set; }
    }
    public class FeeReponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public Fee fee { get; set; }
    }
    public class Fee
    {
        public string name { get; set; }
        public int fee { get; set; }
        public int insurance_fee { get; set; }
        public bool delivery { get; set; }
    }
    public class productsShip
    {
        public string name { get; set; }
        public int? price { get; set; }
        public double? weight { get; set; }
        public int? quantity { get; set; }
        public int product_code { get; set; }
    }
    public class orderrequest
    {
        public string id { get; set; }
        public string pick_name { get; set; }
        public int pick_money { get; set; }
        public string pick_address { get; set; }
        public string pick_province { get; set; }
        public string pick_district { get; set; }
        public string? pick_ward { get; set; }
        public string? pick_street { get; set; }
        public string pick_tel { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string province { get; set; }
        public string district { get; set; }
        public string ward { get; set; }
        public string? street { get; set; }
        public string hamlet { get; set; }
        public string tel { get; set; }
        public int is_freeship { get; set; }
        public int value { get; set; }
        public Array tags { get; set; }


    }
    public class ShipOrderRequest
    {
        public List<productsShip> products { get; set; }
        public orderrequest order { get; set; }
    }
    public class ShipReponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public ShipOrderReponse order { get; set; }
    }
    public class ShipOrderReponse
    {
        public string partner_id { get; set; }
        public string label_id { get; set; }
        public int status { get; set; }
        public string status_text { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string message { get; set; }
        public string pick_date { get; set; }
        public string deliver_date { get; set; }
        public string customer_fullname { get; set; }
        public string customer_tel { get; set; }
        public string address { get; set; }
        public int storage_day { get; set; }
        public int ship_money { get; set; }
        public int insurance { get; set; }
        public int value { get; set; }
        public int weight { get; set; }
        public int pick_money { get; set; }
        public int is_freeship { get; set; }
    }
}
