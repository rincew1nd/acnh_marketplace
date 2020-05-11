using ACNH_Marketplace.DataBase;
using ACNH_Marketplace.Telegram.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ACNH_Marketplace.Web.Controllers
{
    [Route("api/telegram")]
    public class TelegramController : ControllerBase
    {
        private readonly IBotUpdateService _botUpdate;

        public TelegramController(IBotUpdateService botUpdate)
        {
            _botUpdate = botUpdate;
        }

        [HttpPost, Route("update")]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            await _botUpdate.ProceedUpdate(update);
            return Ok();
        }
    }
}