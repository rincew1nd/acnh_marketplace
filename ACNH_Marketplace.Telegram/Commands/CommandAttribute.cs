using System;
using System.Collections.Generic;
using System.Text;

namespace ACNH_Marketplace.Telegram.Commands
{
    class CommandAttribute : Attribute
    {
        public string Regex;

        public CommandAttribute(string regex)
        {
            Regex = regex;
        }
    }
}
