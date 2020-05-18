// <copyright file="UserProfileCommand.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Commands
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ACNH_Marketplace.DataBase;
    using ACNH_Marketplace.DataBase.Enums;
    using ACNH_Marketplace.DataBase.Models;
    using ACNH_Marketplace.Telegram.Commands.CommandBase;
    using ACNH_Marketplace.Telegram.Enums;
    using ACNH_Marketplace.Telegram.Services.BotService;
    using global::Telegram.Bot.Types.ReplyMarkups;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// User profile command processor.
    /// </summary>
    public class UserProfileCommand : BaseCommand
    {
        private const string WelcomeMessage = @"Befor we start, there are some data you should provide:
1. Your current in game name (IGN) and island name.
2. Your current timezone in UTC format for correct date handling between multiple timezones.";

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileCommand"/> class.
        /// </summary>
        /// <param name="telegramBot"><see cref="ITelegramBotService"/>.</param>
        /// <param name="context"><see cref="MarketplaceContext"/>.</param>
        public UserProfileCommand(ITelegramBotService telegramBot, MarketplaceContext context)
            : base(telegramBot, context)
        {
        }

        private UserStateEnum UserState { get; set; }

        private PersonifiedUpdate Update { get; set; }

        /// <inheritdoc/>
        public override async Task<OperationExecutionResult> Execute(PersonifiedUpdate update)
        {
            this.Update = update;
            this.UserState = update.UserContext.GetContext<UserStateEnum>(UserContextEnum.UserState);

            switch (this.UserState)
            {
                case UserStateEnum.NotRegistered:
                    await this.RegistrateUser();
                    break;
                case UserStateEnum.ProfileMain:
                    if (update.Command == "/BackMainMenu")
                    {
                        update.UserContext.SetContext(UserContextEnum.UserState, UserStateEnum.MainPage);
                        return OperationExecutionResult.Reroute;
                    }
                    else if (update.Command.StartsWith("/Change"))
                    {
                        await this.ProfileEdit();
                    }
                    else
                    {
                        await this.ProfileMain();
                    }

                    break;
                case UserStateEnum.ProfileEdit:
                    if (update.Command == "/BackMainMenu")
                    {
                        update.UserContext.SetContext(UserContextEnum.UserState, UserStateEnum.MainPage);
                        return OperationExecutionResult.Reroute;
                    }
                    else if (update.Command == "/ProfileMain")
                    {
                        await this.ProfileMain();
                    }
                    else
                    {
                        await this.ProfileEdit();
                    }

                    break;
            }

            return OperationExecutionResult.Success;
        }

        #region Operations
        private async Task ProfileMain()
        {
            this.Update.UserContext.SetContext(UserContextEnum.UserState, UserStateEnum.ProfileMain);

            await this.Client.EditMessageAsync(
                this.Update.UserContext.TelegramId,
                this.Update.MessageId,
                await this.GetProfileInfo(),
                replyMarkup: new InlineKeyboardMarkup(
                    new[]
                    {
                        new[]
                        {
                            new InlineKeyboardButton() { CallbackData = "/ChangeIGN", Text = "Change IGN" },
                            new InlineKeyboardButton() { CallbackData = "/ChangeIN", Text = "Change island name" },
                        },
                        new[]
                        {
                            new InlineKeyboardButton() { CallbackData = "/ChangeTZ", Text = "Change timezone" },
                            new InlineKeyboardButton() { CallbackData = "/ChangeContacts", Text = "Change contacts" },
                        },
                        new[] { new InlineKeyboardButton() { CallbackData = "/BackMainMenu", Text = "<- Back" } },
                    }));
            return;
        }

        private async Task ProfileEdit()
        {
            var result = false;
            this.Update.UserContext.SetContext(UserContextEnum.UserState, UserStateEnum.ProfileEdit);

            if (this.Update.Command == "/ChangeIGN")
            {
                await this.EnteringIGN();
                this.Update.UserContext.SetContext("ProfileEditType", 1);
                return;
            }

            if (this.Update.Command == "/ChangeIN")
            {
                await this.EnteringIGN();
                this.Update.UserContext.SetContext("ProfileEditType", 2);
                return;
            }

            if (this.Update.Command == "/ChangeTZ")
            {
                await this.EnteringIGN();
                this.Update.UserContext.SetContext("ProfileEditType", 3);
                return;
            }

            if (this.Update.Command == "/ChangeContacts")
            {
                await this.ProfileContactTypes();
                this.Update.UserContext.SetContext("ProfileEditType", 4);
                return;
            }

            switch (this.Update.UserContext.GetContext<int>("ProfileEditType"))
            {
                case 1:
                    result = await this.ValidateIGN();
                    break;
                case 2:
                    result = await this.ValidateIslandName();
                    break;
                case 3:
                    result = await this.ValidateTimezone();
                    break;
                case 4:
                    await this.EnteringContact();
                    return;
                case 5:
                    if (await this.ValidateContact())
                    {
                        await this.UpdateSocial();
                    }

                    break;
            }

            if (result)
            {
                await this.UpdateUser();
            }
        }

        private async Task ProfileContactTypes()
        {
            await this.Client.EditMessageAsync(
                this.Update.UserContext.TelegramId,
                this.Update.MessageId,
                "Choose contact type:",
                new InlineKeyboardMarkup(
                    new[]
                    {
                        new[]
                        {
                            new InlineKeyboardButton() { CallbackData = "/ChangeReddit", Text = "Change Reddit" },
                            new InlineKeyboardButton() { CallbackData = "/ChangeDiscord", Text = "Change Discord" },
                        },
                        new[]
                        {
                            new InlineKeyboardButton() { CallbackData = "/ChangeTwitter", Text = "Change Twitter" },
                            new InlineKeyboardButton() { CallbackData = "/ChangeFacebook", Text = "Change Facebook" },
                        },
                        new[] { new InlineKeyboardButton() { CallbackData = "/ProfileMain", Text = "<- Back" } },
                    }));
        }

        private async Task RegistrateUser()
        {
            var registrationState = this.Update.UserContext.GetContext<int>("RegistrationState");
            switch (registrationState)
            {
                case 0:
                    await this.Client.SendMessageAsync(this.Update.UserContext.TelegramId, WelcomeMessage);
                    await this.EnteringIGN();
                    break;
                case 1:
                    if (await this.ValidateIGN())
                    {
                        await this.EnteringIslandName();
                    }

                    break;
                case 2:
                    if (await this.ValidateIslandName())
                    {
                        await this.EnteringTimezone();
                    }

                    break;
                case 3:
                    if (await this.ValidateTimezone())
                    {
                        await this.UpdateUser();
                    }

                    break;
            }
        }
        #endregion

        #region Entering data messages
        private async Task EnteringIGN()
        {
            await this.Client.EditMessageAsync(
                this.Update.UserContext.TelegramId,
                this.Update.MessageId,
                "Please enter in game name (IGN):");
            this.Update.UserContext.SetContext("RegistrationState", 1);
        }

        private async Task EnteringIslandName()
        {
            await this.Client.EditMessageAsync(
                this.Update.UserContext.TelegramId,
                this.Update.MessageId,
                "Please enter island name:");
            this.Update.UserContext.SetContext("RegistrationState", 2);
        }

        private async Task EnteringTimezone()
        {
            await this.Client.EditMessageAsync(
                this.Update.UserContext.TelegramId,
                this.Update.MessageId,
                "Please enter timezone (from -14 to 14):");
            this.Update.UserContext.SetContext("RegistrationState", 3);
        }

        private async Task EnteringContact()
        {
            switch (this.Update.Command)
            {
                case "/ChangeReddit":
                    this.Update.UserContext.SetContext("UserContactType", UserContactType.Reddit);
                    await this.Client.EditMessageAsync(this.Update.UserContext.TelegramId, this.Update.MessageId, "Enter Reddit username:");
                    break;
                case "/ChangeDiscord":
                    this.Update.UserContext.SetContext("UserContactType", UserContactType.Discord);
                    await this.Client.EditMessageAsync(this.Update.UserContext.TelegramId, this.Update.MessageId, "Enter Discord username:");
                    break;
                case "/ChangeTwitter":
                    this.Update.UserContext.SetContext("UserContactType", UserContactType.Twitter);
                    await this.Client.EditMessageAsync(this.Update.UserContext.TelegramId, this.Update.MessageId, "Enter Twitter username:");
                    break;
                case "/ChangeFacebook":
                    this.Update.UserContext.SetContext("UserContactType", UserContactType.Facebook);
                    await this.Client.EditMessageAsync(this.Update.UserContext.TelegramId, this.Update.MessageId, "Enter Facebook username:");
                    break;
            }

            this.Update.UserContext.SetContext("ProfileEditType", 5);
        }
        #endregion

        #region Validate user data
        private async Task<bool> ValidateIGN()
        {
            if (string.IsNullOrWhiteSpace(this.Update.Command))
            {
                await this.Client.SendMessageAsync(this.Update.UserContext.TelegramId, "IGN should not be empty.");
                await this.EnteringIGN();
                return false;
            }

            if (this.Update.Command.Length > 10)
            {
                await this.Client.SendMessageAsync(
                    this.Update.UserContext.TelegramId,
                    "IGN should not contains more than 10 symbols.");
                await this.EnteringIGN();
                return false;
            }

            this.Update.UserContext.SetContext("IGN", this.Update.Command);
            return true;
        }

        private async Task<bool> ValidateIslandName()
        {
            if (string.IsNullOrWhiteSpace(this.Update.Command))
            {
                await this.Client.SendMessageAsync(
                    this.Update.UserContext.TelegramId,
                    "Island name should not be empty.");
                await this.EnteringIslandName();
                return false;
            }

            if (this.Update.Command.Length > 10)
            {
                await this.Client.SendMessageAsync(
                    this.Update.UserContext.TelegramId,
                    "Island name should not contains more than 10 symbols.");
                await this.EnteringIslandName();
                return false;
            }

            this.Update.UserContext.SetContext("IslandName", this.Update.Command);
            return true;
        }

        private async Task<bool> ValidateTimezone()
        {
            if (!int.TryParse(this.Update.Command, out var timezone))
            {
                await this.Client.SendMessageAsync(
                    this.Update.UserContext.TelegramId,
                    "Entered not valid timezone. Only numbers available.");
                await this.EnteringTimezone();
                return false;
            }

            if (timezone < -14 || timezone > 14)
            {
                await this.Client.SendMessageAsync(
                    this.Update.UserContext.TelegramId,
                    "Timezones limited to more than -14 and less than 14.");
                await this.EnteringTimezone();
                return false;
            }

            this.Update.UserContext.Timezone = timezone;
            return true;
        }

        private async Task<bool> ValidateContact()
        {
            if (string.IsNullOrWhiteSpace(this.Update.Command))
            {
                await this.Client.SendMessageAsync(
                    this.Update.UserContext.TelegramId,
                    "Island name should not be empty.");
                await this.EnteringIGN();
                return false;
            }

            return true;
        }
        #endregion

        #region Database operations
        private async Task<string> GetProfileInfo()
        {
            var user = await this.Context.Users
                .Include(u => u.UserContacts)
                .FirstOrDefaultAsync(u => u.TelegramId == this.Update.UserContext.TelegramId);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Unregistered user accessed profile page");
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine($"In Game Name: {user.InGameName}");
                sb.AppendLine($"Island Name: {user.IslandName}");
                sb.AppendLine($"Timezone: UTC{user.Timezone}");
                sb.AppendLine($"Contacts:");
                sb.AppendLine($"\t{string.Join("\n\t", user.UserContacts.Select(uc => $"{uc.Type} - {uc.Contact}"))}");
                sb.AppendLine("\nWhat do you wish to change?");
                return sb.ToString();
            }
        }

        private async Task UpdateUser()
        {
            var ign = this.Update.UserContext.GetContext<string>("IGN");
            var island = this.Update.UserContext.GetContext<string>("IslandName");
            var timezone = this.Update.UserContext.Timezone;

            var user = await this.Context.Users
                .FirstOrDefaultAsync(u => u.TelegramId == this.Update.UserContext.TelegramId);

            if (user == null)
            {
                user = new User()
                {
                    TelegramId = this.Update.UserContext.TelegramId,
                    InGameName = ign,
                    IslandName = island,
                    Timezone = timezone,
                };
                await this.Context.AddAsync(user);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(ign))
                {
                    user.InGameName = ign;
                }

                if (!string.IsNullOrWhiteSpace(island))
                {
                    user.IslandName = island;
                }

                if (timezone != 0)
                {
                    user.Timezone = timezone;
                }

                this.Context.Update(user);
            }

            await this.Context.SaveChangesAsync();

            this.Update.UserContext.RemoveContext("ProfileEditType");

            await this.ProfileMain();
        }

        private async Task UpdateSocial()
        {
            var type = this.Update.UserContext.GetContext<UserContactType>("UserContactType");
            var contact = this.Update.Command;

            var user = await this.Context.Users
                .Include(u => u.UserContacts)
                .FirstOrDefaultAsync(u => u.TelegramId == this.Update.UserContext.TelegramId);
            UserContact userContacts = user.UserContacts.FirstOrDefault(uc => uc.Type == type);

            if (userContacts == null)
            {
                userContacts = new UserContact()
                {
                    UserId = user.Id,
                    Type = type,
                    Contact = contact,
                };
                await this.Context.AddAsync(userContacts);
            }
            else
            {
                userContacts.Contact = contact;
                this.Context.Update(userContacts);
            }

            await this.Context.SaveChangesAsync();
            this.Update.UserContext.RemoveContext("UserContactType");
            this.Update.UserContext.RemoveContext("ProfileEditType");

            await this.ProfileMain();
        }
        #endregion
    }
}
