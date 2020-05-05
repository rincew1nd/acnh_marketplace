using ACNH_Marketplace.DataBase;
using ACNH_Marketplace.DataBase.Models;
using ACNH_Marketplace.Telegram.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;

namespace ACNH_Marketplace.Telegram.Helpers
{
    static class CommandHelpers
    {
        public static Type[] GetCommand(string command, bool onlyOne = true)
        {
            var types = new List<Type>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var attribute = type.GetCustomAttribute<CommandAttribute>(false);
                if (attribute != null)
                {
                    if (Regex.IsMatch(command, attribute.Regex))
                    {
                        types.Add(type);
                    }
                }
            }

            if (!types.Any())
                throw new CommandNotFoundException();

            if (types.Count > 1 && onlyOne)
                throw new ApplicationException("Found more than one types");

            return types.ToArray();
        }

        public static Type[] FindAllCommands()
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

        public static BaseCommand CreateCommand(Type type, User user, TelegramBotClient client, MarketplaceContext context)
        {
            return (BaseCommand) Activator.CreateInstance(type, client, context, user);
        }
    }
}
