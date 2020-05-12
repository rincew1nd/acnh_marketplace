using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ACNH_Marketplace.Telegram.Services
{
    public interface IBotUpdateService
    {
        Task ProceedUpdate(PersonifiedUpdate update);
    }
}