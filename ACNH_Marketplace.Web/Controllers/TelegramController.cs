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
        private IBotUpdateService _botUpdate;
        private MarketplaceContext _context;

        public TelegramController(IBotUpdateService botUpdate, MarketplaceContext context)
        {
            _botUpdate = botUpdate;
            _context = context;
        }

        [HttpPost, Route("update")]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            await _botUpdate.ProceedUpdate(update);
            return Ok();
        }
    }
}