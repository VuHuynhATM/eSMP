﻿using eSMP.Models;
using eSMP.Services.StoreRepo;
using eSMP.Services.UserRepo;
using eSMP.VModels;
using Firebase.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IStoreReposity _storeReposity;
        private readonly IUserReposity _userReposity;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StoreController(IStoreReposity storeReposity, IHttpContextAccessor httpContextAccessor, IUserReposity userReposity)
        {
            _storeReposity = storeReposity;
            _httpContextAccessor = httpContextAccessor;
            _userReposity = userReposity;
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        public IActionResult GetAllStore(string? search)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                var result = _storeReposity.GetAllStore(search);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "3")]
        [Route("register")]
        public IActionResult RegisterStore([FromForm] StoreRegister store)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                else if (userId != store.UserID + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", });
                }
                var result = _storeReposity.CteateStore(store);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpGet]
        [Route("store_detail")]
        public IActionResult GetStore(int storeID)
        {
            try
            {
                var result = _storeReposity.StoreDetail(storeID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "3")]
        [Route("store_edit")]
        public IActionResult EditStore(StoreUpdateInfo info)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var store = _storeReposity.GetStore(int.Parse(userId));
                if (!_storeReposity.CheckStoreActive(store.StoreID))
                {
                        return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                else if (store != null)
                {
                    if (store.StoreID != info.StoreID)
                    {
                        return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", });
                    }
                }
                var result = _storeReposity.StoreUpdateInfo(info);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("active_store")]
        public IActionResult ActiveStore(int storeID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                return Ok(_storeReposity.ActiveStore(storeID));
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("block_store")]
        public IActionResult BlockStore(int storeID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var store = _storeReposity.GetStore(int.Parse(userId));
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                if (store != null)
                {
                    if (store.StoreID != storeID)
                    {
                        return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", });
                    }
                }
                return Ok(_storeReposity.BlockStore(storeID));
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "3")]
        [Route("hidden_store")]
        public IActionResult HiddenStore(int storeID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var store = _storeReposity.GetStore(int.Parse(userId));
                if (!_storeReposity.CheckStoreActive(store.StoreID))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                else if (store != null)
                {
                    if (store.StoreID != storeID)
                    {
                        return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", });
                    }
                }
                return Ok(_storeReposity.HiddenStore(storeID));
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "3")]
        [Route("unhidden_store")]
        public IActionResult UnHiddenStore(int storeID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var store = _storeReposity.GetStore(int.Parse(userId));
                if (!_storeReposity.CheckStoreActive(store.StoreID))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                else if (store != null)
                {
                    if (store.StoreID != storeID)
                    {
                        return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", });
                    }
                }
                return Ok(_storeReposity.UnHiddenStore(storeID));
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "3")]
        [Route("update_address")]
        public IActionResult UpdateAddress(int storeID, Address address)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var store = _storeReposity.GetStore(int.Parse(userId));
                if (!_storeReposity.CheckStoreActive(store.StoreID))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                else if (store != null)
                {
                    if (store.StoreID != storeID)
                    {
                        return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", });
                    }
                }
                return Ok(_storeReposity.UpdateAddress(storeID, address));
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "3")]
        [Route("login_store")]
        public IActionResult GetStoreByUserID(int userID)
        {
            try
            {
                return Ok(_storeReposity.GetStorebyuserID(userID));
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "AuthDemo")]
        [Route("check_store")]
        public IActionResult CheckStoreFirebase(string firebaseID)
        {
            try
            {
                return Ok(_storeReposity.CheckStoreFirebase(firebaseID));
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
    }
}
