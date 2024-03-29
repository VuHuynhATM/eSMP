﻿using eSMP.Services.NotificationRepo;
using eSMP.VModels;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(AuthenticationSchemes = "AuthDemo")]
        public IActionResult GetNotification(int userID, int? page)
        {
            try
            {
                return Ok(_notification.GetListNotification(userID,page));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = "AuthDemo")]
        public IActionResult pushNotification(FirebaseNotification request)
        {
            try
            {
                return Ok(_notification.PushUserNotificationAsync(request));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
    }
}
