// <copyright file="UserProfileCommand.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Commands
{
    using System.Linq;
    using System.Threading.Tasks;
    using ACNH_Marketplace.DataBase;
    using ACNH_Marketplace.DataBase.Enums;
    using ACNH_Marketplace.DataBase.Models;
    using ACNH_Marketplace.Telegram.Commands.CommandBase;
    using ACNH_Marketplace.Telegram.Enums;
    using ACNH_Marketplace.Telegram.Services;
    using global::Telegram.Bot.Types.ReplyMarkups;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// User profile command processor.
    /// </summary>
    public class UserProfileCommand : BaseCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileCommand"/> class.
        /// </summary>
        /// <param name="telegramBot"><see cref="TelegramBot"/>.</param>
        /// <param name="context"><see cref="MarketplaceContext"/>.</param>
        public UserProfileCommand(TelegramBot telegramBot, MarketplaceContext context)
            : base(telegramBot, context)
        {
        }

        /// <summary>
        /// Gets or sets current user state.
        /// </summary>
        private UserStateEnum UserState { get; set; }

        /// <summary>
        /// Gets or sets current personified update.
        /// </summary>
        private PersonifiedUpdate Update { get; set; }

        /// <inheritdoc/>
        public override async Task Execute(PersonifiedUpdate update)
        {
            this.Update = update;
            this.UserState = update.Context.GetContext<UserStateEnum>(UserContextEnum.UserState);

            switch (this.UserState)
            {
                case UserStateEnum.NotRegistered:
                    await this.RegistrateUser();
                    break;
                case UserStateEnum.ProfileMain:
                    if (string.IsNullOrWhiteSpace(update.Command) || update.Command == "/ProfileMain")
                    {
                        await this.ProfileMain();
                    }
                    else
                    {
                        await this.ProfileEdit();
                    }

                    break;
                case UserStateEnum.ProfileEdit:
                    if (update.Command == "/ProfileMain")
                    {
                        await this.ProfileMain();
                    }
                    else
                    {
                        await this.ProfileEdit();
                    }

                    break;
            }

            return;
        }

        #region Operations
        private async Task ProfileMain()
        {
            this.Update.Context.SetContext(UserContextEnum.UserState, UserStateEnum.ProfileMain);
            await this.Client.SendMessageAsync(
                this.Update.Context.UserId,
                "Profile management page.\nChose operation:",
                replyMarkup: new InlineKeyboardMarkup(
                    new[]
                    {
                        new[] { new InlineKeyboardButton() { CallbackData = "/ChangeIGN", Text = "Change IGN" } },
                        new[] { new InlineKeyboardButton() { CallbackData = "/ChangeIN", Text = "Change island name" } },
                        new[] { new InlineKeyboardButton() { CallbackData = "/ChangeTZ", Text = "Change timezone" } },
                        new[] { new InlineKeyboardButton() { CallbackData = "/ChangeContacts", Text = "Change contacts" } },
                        new[] { new InlineKeyboardButton() { CallbackData = "/BackM", Text = "<- Back" } },
                    }));
            return;
        }

        private async Task ProfileEdit()
        {
            var result = false;
            this.Update.Context.SetContext(UserContextEnum.UserState, UserStateEnum.ProfileEdit);

            if (this.Update.Command == "/ChangeIGN")
            {
                await this.EnteringIGN();
                this.Update.Context.SetContext("ProfileEditType", 1);
                return;
            }

            if (this.Update.Command == "/ChangeIN")
            {
                await this.EnteringIGN();
                this.Update.Context.SetContext("ProfileEditType", 2);
                return;
            }

            if (this.Update.Command == "/ChangeTZ")
            {
                await this.EnteringIGN();
                this.Update.Context.SetContext("ProfileEditType", 3);
                return;
            }

            if (this.Update.Command == "/ChangeContacts")
            {
                await this.ProfileContactTypes();
                this.Update.Context.SetContext("ProfileEditType", 4);
                return;
            }

            switch (this.Update.Context.GetContext<int>("ProfileEditType"))
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
            await this.Client.EditMessage(
                this.Update.Context.UserId,
                this.Update.MessageId,
                "Choose contact type:",
                new InlineKeyboardMarkup(
                    new[]
                    {
                        new[] { new InlineKeyboardButton() { CallbackData = "/ChangeReddit", Text = "Change Reddit" } },
                        new[] { new InlineKeyboardButton() { CallbackData = "/ChangeDiscord", Text = "Change Discord" } },
                        new[] { new InlineKeyboardButton() { CallbackData = "/ChangeTwitter", Text = "Change Twitter" } },
                        new[] { new InlineKeyboardButton() { CallbackData = "/ChangeFacebook", Text = "Change Facebook" } },
                        new[] { new InlineKeyboardButton() { CallbackData = "/ProfileMain", Text = "<- Back" } },
                    }));
        }

        private async Task RegistrateUser()
        {
            var registrationState = this.Update.Context.GetContext<int>("RegistrationState");
            switch (registrationState)
            {
                case 0:
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
            await this.Client.EditMessage(
                this.Update.Context.UserId,
                this.Update.MessageId,
                "Please enter in game name (IGN):");
            this.Update.Context.SetContext("RegistrationState", 1);
        }

        private async Task EnteringIslandName()
        {
            await this.Client.EditMessage(
                this.Update.Context.UserId,
                this.Update.MessageId,
                "Please enter island name:");
            this.Update.Context.SetContext("RegistrationState", 2);
        }

        private async Task EnteringTimezone()
        {
            await this.Client.EditMessage(
                this.Update.Context.UserId,
                this.Update.MessageId,
                "Please enter timezone (from -14 to 14):");
            this.Update.Context.SetContext("RegistrationState", 3);
        }

        private async Task EnteringContact()
        {
            switch (this.Update.Command)
            {
                case "/ChangeReddit":
                    this.Update.Context.SetContext("UserContactType", UserContactType.Reddit);
                    await this.Client.EditMessage(this.Update.Context.UserId, this.Update.MessageId, "Enter Reddit username:");
                    break;
                case "/ChangeDiscord":
                    this.Update.Context.SetContext("UserContactType", UserContactType.Discord);
                    await this.Client.EditMessage(this.Update.Context.UserId, this.Update.MessageId, "Enter Discord username:");
                    break;
                case "/ChangeTwitter":
                    this.Update.Context.SetContext("UserContactType", UserContactType.Twitter);
                    await this.Client.EditMessage(this.Update.Context.UserId, this.Update.MessageId, "Enter Twitter username:");
                    break;
                case "/ChangeFacebook":
                    this.Update.Context.SetContext("UserContactType", UserContactType.Facebook);
                    await this.Client.EditMessage(this.Update.Context.UserId, this.Update.MessageId, "Enter Facebook username:");
                    break;
            }
        }
        #endregion

        #region Validate user data
        private async Task<bool> ValidateIGN()
        {
            if (string.IsNullOrWhiteSpace(this.Update.Command))
            {
                await this.Client.SendMessageAsync(this.Update.Context.UserId, "IGN should not be empty.");
                await this.EnteringIGN();
                return false;
            }

            if (this.Update.Command.Length > 10)
            {
                await this.Client.SendMessageAsync(
                    this.Update.Context.UserId,
                    "IGN should not contains more than 10 symbols.");
                await this.EnteringIGN();
                return false;
            }

            this.Update.Context.SetContext("IGN", this.Update.Command);
            return true;
        }

        private async Task<bool> ValidateIslandName()
        {
            if (string.IsNullOrWhiteSpace(this.Update.Command))
            {
                await this.Client.SendMessageAsync(
                    this.Update.Context.UserId,
                    "Island name should not be empty.");
                await this.EnteringIslandName();
                return false;
            }

            if (this.Update.Command.Length > 10)
            {
                await this.Client.SendMessageAsync(
                    this.Update.Context.UserId,
                    "Island name should not contains more than 10 symbols.");
                await this.EnteringIslandName();
                return false;
            }

            this.Update.Context.SetContext("IslandName", this.Update.Command);
            return true;
        }

        private async Task<bool> ValidateTimezone()
        {
            if (!int.TryParse(this.Update.Command, out var timezone))
            {
                await this.Client.SendMessageAsync(
                    this.Update.Context.UserId,
                    "Entered not valid timezone. Only numbers available.");
                await this.EnteringTimezone();
                return false;
            }

            if (timezone < -14 || timezone > 14)
            {
                await this.Client.SendMessageAsync(
                    this.Update.Context.UserId,
                    "Timezones limited to more than -14 and less than 14.");
                await this.EnteringTimezone();
                return false;
            }

            this.Update.Context.SetContext("Timezone", timezone);
            return true;
        }

        private async Task<bool> ValidateContact()
        {
            if (string.IsNullOrWhiteSpace(this.Update.Command))
            {
                await this.Client.SendMessageAsync(
                    this.Update.Context.UserId,
                    "Island name should not be empty.");
                await this.EnteringIGN();
                return false;
            }

            return true;
        }
        #endregion

        #region Database updates
        private async Task UpdateUser()
        {
            var ign = this.Update.Context.GetContext<string>("IGN");
            var island = this.Update.Context.GetContext<string>("IslandName");
            var timezone = this.Update.Context.GetContext<int>("Timezone");

            var user = await this.Context.Users
                .FirstOrDefaultAsync(u => u.TelegramId == this.Update.Context.UserId);

            if (user == null)
            {
                user = new User()
                {
                    TelegramId = this.Update.Context.UserId,
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
                    user.InGameName = ign;
                }

                if (timezone != 0)
                {
                    user.Timezone = timezone;
                }

                this.Context.Update(user);
            }

            await this.Context.SaveChangesAsync();

            this.Update.Context.RemoveContext("IGN");
            this.Update.Context.RemoveContext("IslandName");
            this.Update.Context.RemoveContext("Timezone");
            this.Update.Context.RemoveContext("ProfileEditType");

            await this.ProfileMain();
        }

        private async Task UpdateSocial()
        {
            var type = this.Update.Context.GetContext<UserContactType>("UserContactType");
            var contact = this.Update.Context.GetContext<string>("UserContact");

            var user = await this.Context.Users
                .Include(u => u.UserContacts)
                .FirstOrDefaultAsync(u => u.TelegramId == this.Update.Context.UserId);
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
            this.Update.Context.RemoveContext("UserContact");
            this.Update.Context.RemoveContext("UserContactType");
            this.Update.Context.RemoveContext("ProfileEditType");
        }
        #endregion
    }
}
