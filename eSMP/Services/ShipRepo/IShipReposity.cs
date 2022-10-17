using eSMP.VModels;
using Microsoft.AspNetCore.Mvc;

namespace eSMP.Services.ShipRepo
{
    public interface IShipReposity
    {
        public Fee GetFeeAsync(string province, string district, string pick_province, string pick_district, int weight);
    }
}
