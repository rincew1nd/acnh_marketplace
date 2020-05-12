using System;

namespace ACNH_Marketplace.Telegram.Services
{
    public interface ICommandRouterService
    {
        Type FindCommand(PersonifiedUpdate update);
    }
}