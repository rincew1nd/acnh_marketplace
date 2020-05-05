using ACNH_Marketplace.DataBase;
using ACNH_Marketplace.Telegram.Commands;
using ACNH_Marketplace.Telegram.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using User = ACNH_Marketplace.DataBase.Models.User;

namespace ACNH_Marketplace.Telegram
{
    public class MarketplaceBot
    {
        private TelegramBotClient _client;
        private readonly IServiceScopeFactory _scopeFactory;
        private ILogger _logger;

        public MarketplaceBot(ILogger<MarketplaceBot> logger, BotConfiguration config, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;

#if (DEBUG)
            _client = new TelegramBotClient(
                config.Token,
                new WebProxy(config.Proxy.Address, config.Proxy.Port)
                {
                    Credentials = new NetworkCredential(config.Proxy.User, config.Proxy.Password)
                }
            );
#elif (RELEASE)
            _client = new TelegramBotClient(
                config.Token
            );
#endif

            InitCommands();
        }

        public void InitCommands()
        {
            var me = _client.GetMeAsync().Result;
            _logger.LogInformation(
              $"Bot {me.Id}-{me.FirstName} is active."
            );

            _client.OnMessage += Bot_OnMessage;
            _client.OnMessageEdited += Bot_OnMessageEdited;
            _client.OnInlineQuery += Bot_OnInlineQuery;
            _client.OnInlineResultChosen += Bot_OnInlineResultChosen;
            _client.OnCallbackQuery += Bot_OnCallbackQueryReceived;
            _client.OnReceiveError += Bot_OnReceiveError;
        }

        public void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            SafeExecution(async (command) =>
            {
                await command.Execute(e, false);
            }, e.Message.From.Id, e.Message.Text);
        }

        private void Bot_OnMessageEdited(object sender, MessageEventArgs e)
        {
            SafeExecution(async (command) =>
            {
                await command.Execute(e, true);
            }, e.Message.From.Id, e.Message.Text);
        }

        private void Bot_OnInlineQuery(object sender, InlineQueryEventArgs e)
        {
            SafeExecution(async (command) =>
            {
                await command.Execute(e);
            }, e.InlineQuery.From.Id, e.InlineQuery.Query);
        }

        private void Bot_OnInlineResultChosen(object sender, ChosenInlineResultEventArgs e)
        {
            SafeExecution(async (command) =>
            {
                await command.Execute(e);
            }, e.ChosenInlineResult.From.Id, e.ChosenInlineResult.Query);
        }

        private void Bot_OnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            SafeExecution(async (command) =>
            {
                await command.Execute(e);
            }, e.CallbackQuery.From.Id, e.CallbackQuery.Message.Text);
        }

        private void Bot_OnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            SafeExecution(async (command) =>
            {
                await command.Execute(e);
            }, null, e.ApiRequestException.Message);
        }

        public async void SafeExecution(Action<BaseCommand> action, int? userId, string command)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<MarketplaceContext>();

                    var user = userId.HasValue ? await context.Users.FindAsync(userId.Value) : null;
                    var type = CommandHelpers.GetCommand(command)[0];
                    var commandObj = CommandHelpers.CreateCommand(type, user, _client, context);
                    action.Invoke(commandObj);
                }
            }
            catch (Exception ex)
            {
                if (ex is CommandNotFoundException) { }
                else
                    _logger.LogError(ex, "Unhandled exception");
            }
        }

        public void StartReceiving(CancellationToken ct)
        {
            _client.StartReceiving(
                new[] {
                    UpdateType.Message, UpdateType.InlineQuery, UpdateType.CallbackQuery,
                    UpdateType.ChosenInlineResult, UpdateType.EditedMessage
                },
                ct
            );
        }

        public void StopReceiving()
        {
            _client.StopReceiving();
        }
    }
}
