using eSMP.Services.NotificationRepo;
using eSMP.VModels;
using Microsoft.AspNetCore.Mvc;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationReposity _notification;

        public NotificationController(INotificationReposity notification)
        {
            _notification = notification;
        }
        [HttpGet]
        public IActionResult GetNotification(int userID, int? page)
        {
            try
            {
                return Ok(_notification.GetListNotification(userID,page));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
    }
}
