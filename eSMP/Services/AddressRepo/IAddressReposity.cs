using eSMP.VModels;

namespace eSMP.Services.AddressRepo
{
    public interface IAddressReposity
    {
        public Result GetProvines();
        public Result GetDistrict(string tpid);
        public Result GetWard(string qhid);
    }
}
