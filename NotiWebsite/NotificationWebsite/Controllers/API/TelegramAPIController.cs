using Hangfire;
using Microsoft.AspNetCore.Mvc;
using NotificationWebsite.Models;
using NotificationWebsite.Models.Contracts;
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
        private readonly ITelegramBotClient _telegramBotClient;

        private readonly IJwtService _jwtService;

        private readonly INotificationActions _notiActions;

        public TelegramAPIController(ITelegramBotClient _telegramBot, IJwtService jwtService, INotificationActions notiActions)
        {
            _telegramBotClient = _telegramBot;
            _jwtService = jwtService;
            _notiActions = notiActions;
        }
        [HttpPost("telegramSendMessage")]
        public async Task<IActionResult> SendTelegramMessage([FromBody]CreateNotificationRequest request, [FromServices] IHttpContextAccessor accessor)
        {
            if (request == null)
            {
                return BadRequest("Invalid request");
            }

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

                BackgroundJob.Schedule(() => _notiActions.SendAndUpdateNotificationTelegram(chatId, user, notification, _telegramBotClient), delay);
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }

            return Ok("The message was sent successfully");
        }

    }
}