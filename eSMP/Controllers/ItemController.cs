using eSMP.Models;
using eSMP.Services.ItemRepo;
using eSMP.VModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemReposity _itemReposity;

        public ItemController(IItemReposity itemReposity)
        {
            _itemReposity = itemReposity;
        }
        [HttpGet]
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
        }
        [HttpPut]
        [Route("update_subitem")]
        public IActionResult UpdatesubItem(SubItemUpdate subItem)
        {
            try
            {
                return Ok(_itemReposity.UpdatesubItem(subItem));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpGet]
        [Route("store")]
        public IActionResult GetItemWithStatusIDS(int storeID, int? statusID, int page)
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
        public IActionResult GetAllItem(int itemID)
        {
            try
            {
                var result = _itemReposity.GetItemDetail(itemID);
                return Ok(result);
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        
        [HttpDelete]
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
        }
        [HttpPost]
        public IActionResult CreateItem(ItemRegister item)
        {
            try
            {
                var result = _itemReposity.CreateItem(item);
                return Ok(result);
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPost]
        [Route("add_subitem")]
        public IActionResult AddSubItem(int itemID, Sub_ItemRegister SubItem)
        {
            try
            {
                var result = _itemReposity.AddsubItem(itemID,SubItem);
                return Ok(result);
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
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
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPut]
        [Route("active_item")]
        public IActionResult ActiveItem(int itemID)
        {
            try
            {
                return Ok(_itemReposity.ActiveItem(itemID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPut]
        [Route("active_subItem")]
        public IActionResult ActiveSubItem(int subItemID)
        {
            try
            {
                return Ok(_itemReposity.ActivesubItem(subItemID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPut]
        [Route("block_item")]
        public IActionResult BlockItem(int itemID)
        {
            try
            {
                return Ok(_itemReposity.BlockItem(itemID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPut]
        [Route("block_subItem")]
        public IActionResult BlockSubItem(int subItemID)
        {
            try
            {
                return Ok(_itemReposity.BlocksubItem(subItemID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPut]
        [Route("hidden_item")]
        public IActionResult HiddenItem(int itemID)
        {
            try
            {
                return Ok(_itemReposity.HiddenItem(itemID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPut]
        [Route("hiden_subItem")]
        public IActionResult HiddenSubItem(int subItemID)
        {
            try
            {
                return Ok(_itemReposity.HiddensubItem(subItemID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPut]
        [Route("unhidden_item")]
        public IActionResult UnHiddenItem(int itemID)
        {
            try
            {
                return Ok(_itemReposity.UnHiddenItem(itemID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPut]
        [Route("unhiden_subItem")]
        public IActionResult UnHiddenSubItem(int subItemID)
        {
            try
            {
                return Ok(_itemReposity.UnHiddensubItem(subItemID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPut]
        [Route("update_model")]
        public IActionResult UpdateModel(int ItemID, int[] brandModelIDs)
        {
            try
            {
                return Ok(_itemReposity.UpdateBrandModel(ItemID,brandModelIDs));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPut]
        [Route("remove_model")]
        public IActionResult RemoveModel(int ItemID, int[] brandModelIDs)
        {
            try
            {
                return Ok(_itemReposity.RemoveBrandModel(ItemID,brandModelIDs));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }

    }
}
