using eSMP.Models;
using eSMP.VModels;
using System.Collections;

namespace eSMP.Services.AddressRepo
{
    public class Addressrepository:IAddressReposity
    {
        private readonly WebContext _context;

        public Addressrepository(WebContext context)
        {
            _context = context;
        }

        public Result GetDistrict(string tpid)
        {
            Result result = new Result();
            try
            {
                List<AddressVnModel> list = new List<AddressVnModel>();
                var listadd = _context.addressVns.Where(a => a.tpid == tpid).ToList();
                string id = "";
                foreach (var vn in listadd)
                {

                    AddressVnModel addressVn = new AddressVnModel
                    {
                        Key = vn.qhid,
                        Value = vn.district,
                    };
                    if (list.Count == 0)
                    {
                        list.Add(addressVn);
                        id = vn.qhid;
                    }
                    else
                    {
                        if (id != addressVn.Key)
                        {
                            list.Add(addressVn);
                            id = vn.qhid;
                        }
                    }
                }
                result.Success = true;
                result.Message = "Thành công";
                result.Data = list;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                return result;
            }
        }

        public Result GetProvines()
        {
            Result result = new Result();
            try
            {
                List< AddressVnModel>list = new List<AddressVnModel>();
                var listadd = _context.addressVns.ToList();
                string id = "";
                foreach (var vn in listadd)
                {
                    
                    AddressVnModel addressVn = new AddressVnModel
                    {
                        Key=vn.tpid,
                        Value=vn.province,
                    };
                    if (list.Count == 0)
                    {
                        list.Add(addressVn);
                        id = vn.tpid;
                    }
                    else
                    {
                        if (id!= addressVn.Key)
                        {
                            list.Add(addressVn);
                            id = vn.tpid;
                        }
                    }
                }
                result.Success = true;
                result.Message = "Thành công";
                result.Data = list;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                return result;
            }
        }

        public Result GetWard(string qhid)
        {
            Result result = new Result();
            try
            {
                List<AddressVnModel> list = new List<AddressVnModel>();
                var listadd = _context.addressVns.Where(a=>a.qhid==qhid).ToList();
                string id = "";
                foreach (var vn in listadd)
                {

                    AddressVnModel addressVn = new AddressVnModel
                    {
                        Key = vn.id,
                        Value = vn.ward,
                    };
                    if (list.Count == 0)
                    {
                        list.Add(addressVn);
                        id = vn.id;
                    }
                    else
                    {
                        if (id != addressVn.Key)
                        {
                            list.Add(addressVn);
                            id = vn.id;
                        }
                    }
                }
                result.Success = true;
                result.Message = "Thành công";
                result.Data = list;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                return result;
            }
        }
    }
}
