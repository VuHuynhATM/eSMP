﻿using eSMP.Models;
using eSMP.Services.StoreRepo;
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
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
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
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
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
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
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
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Route("active_store")]
        public IActionResult ActiveStore(int storeID)
        {
            try
            {
                return Ok(_sttoreReposity.ActiveStore(storeID));
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Route("block_store")]
        public IActionResult BlockStore(int storeID)
        {
            try
            {
                return Ok(_sttoreReposity.BlockStore(storeID));
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Route("hidden_store")]
        public IActionResult HiddenStore(int storeID)
        {
            try
            {
                return Ok(_sttoreReposity.HiddenStore(storeID));
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Route("unhidden_store")]
        public IActionResult UnHiddenStore(int storeID)
        {
            try
            {
                return Ok(_sttoreReposity.UnHiddenStore(storeID));
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Route("update_address")]
        public IActionResult UpdateAddress(int storeID, Address address)
        {
            try
            {
                return Ok(_sttoreReposity.UpdateAddress(storeID, address));
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
    }
}
