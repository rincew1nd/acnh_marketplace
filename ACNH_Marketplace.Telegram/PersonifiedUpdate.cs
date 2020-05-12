using ACNH_Marketplace.Telegram.Services;
using Telegram.Bot.Types;

namespace ACNH_Marketplace.Telegram
{
    public class PersonifiedUpdate : Update
    {
        public UserContext Context;
        public string Command;
    }
}
