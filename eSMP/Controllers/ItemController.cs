using eSMP.Models;
using eSMP.Services.ItemRepo;
using eSMP.Services.StoreRepo;
using eSMP.Services.UserRepo;
using eSMP.VModels;
using Firebase.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemReposity _itemReposity;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStoreReposity _storeReposity;
        private readonly IUserReposity _userReposity;

        public ItemController(IItemReposity itemReposity, IHttpContextAccessor httpContextAccessor, IStoreReposity storeReposity, IUserReposity userReposity)
        {
            _itemReposity = itemReposity;
            _httpContextAccessor = httpContextAccessor;
            _storeReposity = storeReposity;
            _userReposity = userReposity;
        }
        /*[HttpGet]
        public IActionResult GetItemWithStatusID(int? statusID, int? page)
        {
            try
            {
                var result = _itemReposity.GetItemWithStatusID(statusID, page);
                return Ok(result);
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }*/
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "3")]
        [Route("update_subitem")]
        public IActionResult UpdatesubItem(SubItemUpdate subItem)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var supplierID=_itemReposity.GetSupplierIDBySubItemID(subItem.SubItemID);
                if (!_storeReposity.CheckStoreActive(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (userId != supplierID + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", TotalPage = 1 });
                }
                return Ok(_itemReposity.UpdatesubItem(subItem));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpGet]
        [Route("store")]
        public IActionResult GetItemWithStatusIDS(int storeID, int? statusID, int? page)
        {
            try
            {
                var result = _itemReposity.GetItemWithStatusIDS(storeID, statusID, page);
                return Ok(result);
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpGet]
        [Route("item_detail")]
        public IActionResult GetItemDetail(int itemID)
        {
            try
            {
                var result = _itemReposity.GetItemDetail(itemID);
                return Ok(result);
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpGet]
        [Route("item_feedback")]
        public IActionResult GetListFeedback(int itemID, int? page, int? role)
        {
            try
            {
                var result = _itemReposity.GetListFeedback(itemID,page, role);
                return Ok(result);
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        
        /*[HttpDelete]
        public IActionResult RemoveItem(int itemID)
        {
            try
            {
                var result = _itemReposity.RemoveItem(itemID);
                return Ok(result);
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }*/
        [HttpPost]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles ="3")]
        public IActionResult CreateItem([FromForm]ItemRegister item)
        {
            try
            {
                var role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                if (role != "3")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không có quền thực hiện yêu cầu này", Data = "", TotalPage = 1 });
                }
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var storeID = _storeReposity.GetStore(int.Parse(userId)).StoreID;
                if (!_storeReposity.CheckStoreActive(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (storeID != item.StoreID)
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", TotalPage = 1 });
                }
                var result = _itemReposity.CreateItem(item);
                return Ok(result);
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "3")]
        [Route("add_subitem")]
        public IActionResult AddSubItem([FromForm]Sub_ItemRegister SubItem)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var supplierID = _itemReposity.GetSupplierIDByItemID(SubItem.itemID);
                if (!_storeReposity.CheckStoreActive(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (userId != supplierID + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", TotalPage = 1 });
                }
                var result = _itemReposity.AddsubItem(SubItem);
                return Ok(result);
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpGet]
        [Route("search")]
        public IActionResult SearchItem(string? search, double? min, double? max, double? rate, int? cateID, int? subCateID, int? brandID, int? brandModelID, string? sortBy, double? la, double? lo, int? storeID, int? page)
        {
            try
            {
                var result = _itemReposity.SearchItem(search, min, max, rate, cateID, subCateID, brandID, brandModelID, sortBy, la, lo, storeID, page);
                return Ok(result);
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpGet]
        [Route("search_admin")]
        public IActionResult SearchItemAdmin(string? search, double? min, double? max, double? rate, int? cateID, int? subCateID, int? brandID, int? brandModelID, string? sortBy, double? la, double? lo, int? storeID, int? page, int? itemStatusID)
        {
            try
            {
                var result = _itemReposity.SearchItemForAdmin(search, min, max, rate, cateID, subCateID, brandID, brandModelID, sortBy, la, lo, storeID, page, itemStatusID);
                return Ok(result);
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("active_item")]
        public IActionResult ActiveItem(int itemID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                return Ok(_itemReposity.ActiveItem(itemID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("active_subItem")]
        public IActionResult ActiveSubItem(int subItemID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                return Ok(_itemReposity.ActivesubItem(subItemID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("block_item")]
        public IActionResult BlockItem(int itemID, string? statusText)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                return Ok(_itemReposity.BlockItem(itemID, statusText));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("block_subItem")]
        public IActionResult BlockSubItem(int subItemID, string? statusText)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                return Ok(_itemReposity.BlocksubItem(subItemID, statusText));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "3")]
        [Route("hidden_item")]
        public IActionResult HiddenItem(int itemID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var supplierID = _itemReposity.GetSupplierIDByItemID(itemID);
                if (!_storeReposity.CheckStoreActive(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (userId != supplierID + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", TotalPage = 1 });
                }
                return Ok(_itemReposity.HiddenItem(itemID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "3")]
        [Route("hiden_subItem")]
        public IActionResult HiddenSubItem(int subItemID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var supplierID = _itemReposity.GetSupplierIDBySubItemID(subItemID);
                if (!_storeReposity.CheckStoreActive(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (userId != supplierID + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", TotalPage = 1 });
                }
                return Ok(_itemReposity.HiddensubItem(subItemID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "3")]
        [Route("unhidden_item")]
        public IActionResult UnHiddenItem(int itemID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var supplierID = _itemReposity.GetSupplierIDByItemID(itemID);
                if (!_storeReposity.CheckStoreActive(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (userId != supplierID + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", TotalPage = 1 });
                }
                return Ok(_itemReposity.UnHiddenItem(itemID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "3")]
        [Route("unhiden_subItem")]
        public IActionResult UnHiddenSubItem(int subItemID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var supplierID = _itemReposity.GetSupplierIDBySubItemID(subItemID);
                if (!_storeReposity.CheckStoreActive(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (userId != supplierID + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", TotalPage = 1 });
                }
                return Ok(_itemReposity.UnHiddensubItem(subItemID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "3")]
        [Route("update_model")]
        public IActionResult UpdateModel(int itemID, int[] brandModelIDs)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var supplierID = _itemReposity.GetSupplierIDByItemID(itemID);
                if (!_storeReposity.CheckStoreActive(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (userId != supplierID + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", TotalPage = 1 });
                }
                return Ok(_itemReposity.UpdateBrandModel(itemID,brandModelIDs));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "3")]
        [Route("remove_model")]
        public IActionResult RemoveModel(int itemID, int[] brandModelIDs)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var supplierID = _itemReposity.GetSupplierIDByItemID(itemID);
                if (!_storeReposity.CheckStoreActive(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (userId != supplierID + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", TotalPage = 1 });
                }
                return Ok(_itemReposity.RemoveBrandModel(itemID,brandModelIDs));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
       /* [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "3")]
        [Route("update_discount")]
        public IActionResult UpdateDiscount(int Sub_ItemID, double discount)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var supplierID = _itemReposity.GetSupplierIDBySubItemID(Sub_ItemID);
                if (!_storeReposity.CheckStoreActive(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (userId != supplierID + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", TotalPage = 1 });
                }
                return Ok(_itemReposity.UpdateDiscount(Sub_ItemID, discount));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }*/
    }
}
