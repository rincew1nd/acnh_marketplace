using ACNH_Marketplace.Telegram.Enums;
using System;
using System.Collections.Generic;

namespace ACNH_Marketplace.Telegram.Commands.CommandBase
{
    class CommandAttribute : Attribute
    {
        public readonly Dictionary<UserStateEnum, string> Locators;

        public CommandAttribute(string locators)
        {
            Locators = new Dictionary<UserStateEnum, string>();
            foreach (var locator in locators.Split(';'))
            {
                Locators.Add(
                    (UserStateEnum) Enum.Parse(typeof(UserStateEnum), locator.Split('=')[0]),
                    locator.Split('=')[1]
                );
            }
        }
    }
}
