using ACNH_Marketplace.DataBase;
using ACNH_Marketplace.Telegram._commands.Registration;
using ACNH_Marketplace.Telegram.Commands.CommandBase;
using ACNH_Marketplace.Telegram.Enums;
using ACNH_Marketplace.Telegram.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Telegram.Bot;

namespace ACNH_Marketplace.Telegram.Helpers
{
    static class CommandHelpers
    {
        public static Type[] GetCommandType(UserStateEnum userState, string command, bool onlyOne = true)
        {
            var types = new List<Type>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var attribute = type.GetCustomAttribute<CommandAttribute>(false);
                if (attribute != null)
                {
                    if (attribute.Locators.ContainsKey(userState) &&
                        Regex.IsMatch(command, attribute.Locators[userState]))
                    {
                        types.Add(type);
                    }
                }
            }

            if (!types.Any())
                return new[] { typeof(WelcomeCommand) };

            if (types.Count > 1 && onlyOne)
                throw new ApplicationException("Found more than one types");

            return types.ToArray();
        }

        public static Type[] FindAllCommandTypes()
        {
            var types = new List<Type>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.GetCustomAttributes<CommandAttribute>(false).Any())
                {
                    types.Add(type);
                }
            }

            return types.ToArray();
        }

        public static ICommand CreateCommand(
            Type type, IBotService botService, MarketplaceContext context,
            UserContext userContext, string command)
        {
            return (ICommand) Activator.CreateInstance(type, botService, context, userContext, command);
        }
    }
}
