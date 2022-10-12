using eSMP.Models;
using eSMP.Services;
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
        public IActionResult GetAllItem(int? statusID,int page)
        {
            try
            {
                var result = _itemReposity.GetAllItem(statusID,page);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
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
                return BadRequest();
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
                return BadRequest();
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
                return BadRequest();
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
                return BadRequest();
            }
        }
        [HttpGet]
        [Route("search")]
        public IActionResult SearchItem(string? search, double? min, double? max, double? rate, int? cateID, int? subCateID, int? brandID, int? brandModelID, string? sortBy, double? la, double? lo, int? storeID, int page)
        {
            try
            {
                var result = _itemReposity.SearchItem(search, min, max, rate, cateID, subCateID, brandID, brandModelID, sortBy, la, lo, storeID, page);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
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
    }
}
