using eSMP.Services;
using eSMP.VModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IStoreReposity _sttoreReposity;

        public StoreController(IStoreReposity storeReposity)
        {
            _sttoreReposity = storeReposity;
        }
        [HttpGet]
        public IActionResult GetAllStore()
        {
            try
            {
                var result = _sttoreReposity.GetAllStore();
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("register")]
        public IActionResult RegisterStore(StoreRegister store)
        {
            try
            {
                var result = _sttoreReposity.CteateStore(store);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("store_detail")]
        public IActionResult GetStore(int storeID)
        {
            try
            {
                var result = _sttoreReposity.StoreDetail(storeID);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("store_edit")]
        public IActionResult EditStore(StoreUpdateInfo info)
        {
            try
            {
                var result = _sttoreReposity.StoreUpdateInfo(info);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
