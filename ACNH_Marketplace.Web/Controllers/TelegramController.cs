using ACNH_Marketplace.DataBase;
using ACNH_Marketplace.Telegram;
using ACNH_Marketplace.Telegram.Helpers;
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
        private readonly MarketplaceContext _context;
        private readonly IUserContextService _userContextService;

        public TelegramController(IBotUpdateService botUpdate, MarketplaceContext context, IUserContextService userContextService)
        {
            _botUpdate = botUpdate;
            _context = context;
            _userContextService = userContextService;
        }

        [HttpPost, Route("update")]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            var (userId, command) = UpdateHelpers.GetUserAndCommand(update);
            var user = await _context.Users.FindAsync(userId);
            var userContext = _userContextService.GetUserContext(user, userId);

            PersonifiedUpdate personifiedUpdate = new PersonifiedUpdate()
            {
                Update = update,
                Context = userContext,
                Command = command
            };

            await _botUpdate.ProceedUpdate(personifiedUpdate);
            return Ok();
        }
    }
}