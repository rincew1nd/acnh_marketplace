using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ACNH_Marketplace.Telegram.Commands.CommandBase
{
    public interface ICommand
    {
        Task Execute(PersonifiedUpdate update);
    }
}