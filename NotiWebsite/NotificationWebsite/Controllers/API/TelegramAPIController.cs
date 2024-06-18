using Hangfire;
using Microsoft.AspNetCore.Mvc;
using NotificationWebsite.DataAccess.Data;
using NotificationWebsite.Models;
using NotificationWebsite.Models.Contracts;
using NotificationWebsite.Utility.Configuration;
using NotificationWebsite.Utility.Configuration.TelegramBot;
using NotificationWebsite.Utility.Helpers.NotificationActions;
using NotificationWebsite.Utility.Jwt;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace NotificationWebsite.Controllers.API
{
    [ApiController]
    [Route("api/telegram")]
    public class TelegramAPIController : ControllerBase
    {
        private readonly IJwtService _jwtService;

        private readonly INotificationActions _notiActions;

        ITelegramBotConfiguration _telegramBotConfiguration;

        public TelegramAPIController(IServiceProvider serviceProvider)
        {
            _jwtService = serviceProvider.GetRequiredService<IJwtService>();
            _notiActions = serviceProvider.GetRequiredService<INotificationActions>();
            _telegramBotConfiguration = serviceProvider.GetRequiredService<ITelegramBotConfiguration>();
        }
        [HttpPost("telegramSendMessage")]
        public async Task<IActionResult> SendTelegramMessage([FromBody]CreateNotificationRequest request, [FromServices] IHttpContextAccessor accessor)
        {
            if (request == null)
            {
                return BadRequest("Invalid request");
            }

            _telegramBotConfiguration.Configure();

            Models.User user = await _jwtService.GetUserByTokenAsync(accessor.HttpContext.Request.Cookies["L_Cookie"]);

            Notification notification = _notiActions.MakeNotificationFromRequest(request, user);
            
            if (user.ChatId == 0)
            {
                return NoContent();
            }
            
            try
            {
                var chatId = user.ChatId;
                
                var delay = notification.Date - DateTime.Now;

                await _notiActions.AddNotificationToDBAsync(notification, user);

                BackgroundJob.Schedule(() => _notiActions.SendAndUpdateNotificationTelegram(chatId, user, notification), delay);
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }

            return Ok("The message was sent successfully");
        }

    }
}