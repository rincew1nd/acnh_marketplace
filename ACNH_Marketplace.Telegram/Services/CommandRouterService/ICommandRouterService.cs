using ACNH_Marketplace.DataBase;
using ACNH_Marketplace.Telegram.Commands.CommandBase;

namespace ACNH_Marketplace.Telegram.Services
{
    public interface ICommandRouterService
    {
        ICommand FindCommand(PersonifiedUpdate update);
    }
}