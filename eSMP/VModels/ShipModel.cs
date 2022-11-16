namespace eSMP.VModels
{
    public class ShipModel
    {
        public int orderID { get; set; }
        public string LabelID { get; set; }
        public List<ShipStatusModel> shipStatusModels { get; set; }
    }
    public class ShipStatusModel
    {
        public string Reason_code { get; set; }
        public string Reason { get; set; }
        public string status { get; set; }
        public DateTime Create_Date { get; set; }
    }
    public class ShipViewModel
    {
        public string LabelID { get; set; }
        public string ShipStatusID { get; set; }
        public string Reason_code { get; set; }
        public string Reason { get; set; }
        public string status { get; set; }
        public DateTime Create_Date { get; set; }
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
        public string label { get; set; }
        public int area { get; set; }
        public int fee { get; set; }
        public int insurance_fee { get; set; }
        public string estimated_pick_time { get; set; }
        public string estimated_deliver_time { get; set; }
        public int status_id { get; set; }
    }
    public class ShipINP
    {
        public string label_id { get; set; }
        public string partner_id { get; set; }
        public int status_id { get; set; }
        public string action_time { get; set; }
        public string reason_code { get; set; }
        public string reason { get; set; }
        public float weight { get; set; }
        public int fee { get; set; }
        public int return_part_package { get; set; }
    }

    public class ShipCancel
    {
        public string partner_id { get; set; }
    }
}
