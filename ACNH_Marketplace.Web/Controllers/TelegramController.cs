// <copyright file="TelegramController.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Web.Controllers
{
    using System.Threading.Tasks;
    using ACNH_Marketplace.DataBase;
    using ACNH_Marketplace.Telegram;
    using ACNH_Marketplace.Telegram.Helpers;
    using ACNH_Marketplace.Telegram.Services;
    using global::Telegram.Bot.Types;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Controller for handling Telegram webhook update.
    /// </summary>
    [Route("api/telegram")]
    public class TelegramController : ControllerBase
    {
        private readonly IBotUpdateService botUpdate;
        private readonly MarketplaceContext context;
        private readonly IUserContextService userContextService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramController"/> class.
        /// </summary>
        /// <param name="botUpdate"><see cref="IBotUpdateService"/>.</param>
        /// <param name="context"><see cref="MarketplaceContext"/>.</param>
        /// <param name="userContextService"><see cref="UserContextService"/>.</param>
        public TelegramController(IBotUpdateService botUpdate, MarketplaceContext context, IUserContextService userContextService)
        {
            this.botUpdate = botUpdate;
            this.context = context;
            this.userContextService = userContextService;
        }

        /// <summary>
        /// Webhook for recieving updates from Telegram.
        /// </summary>
        /// <param name="update">Recieved user <see cref="Update">update</see> object.</param>
        /// <returns>Update accepted response.</returns>
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            var (userId, command, messageId) = UpdateHelpers.GetUserAndCommand(update);
            var user = await this.context.Users.FirstOrDefaultAsync(u => u.TelegramId == userId);
            var userContext = this.userContextService.GetUserContext(user, userId);

            PersonifiedUpdate personifiedUpdate = new PersonifiedUpdate()
            {
                Update = update,
                UserContext = userContext,
                Command = command,
                MessageId = messageId,
            };

            await this.botUpdate.ProceedUpdate(personifiedUpdate);
            return this.Ok();
        }
    }
}