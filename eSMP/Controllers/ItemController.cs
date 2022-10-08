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
        public IActionResult GetAllItem()
        {
            try
            {
                var result=_itemReposity.GetAllItem();
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
    }
}
