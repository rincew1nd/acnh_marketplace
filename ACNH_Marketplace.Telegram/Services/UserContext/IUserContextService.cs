using ACNH_Marketplace.DataBase.Models;

namespace ACNH_Marketplace.Telegram.Services
{
    public interface IUserContextService
    {
        UserContext GetUserContext(User user, int userId);
    }
}