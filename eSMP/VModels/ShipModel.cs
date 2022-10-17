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
}
