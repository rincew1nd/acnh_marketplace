using ACNH_Marketplace.DataBase.Models;
using System.Collections.Generic;

namespace ACNH_Marketplace.Telegram
{
    public class UserContextProvider
    {
        private Dictionary<int, UserContext> _userContexts;

        public UserContextProvider()
        {
            _userContexts = new Dictionary<int, UserContext>();
        }

        public UserContext GetUserContext(User user, int userId)
        {
            if (_userContexts.ContainsKey(userId))
            {
                return _userContexts[userId];
            }
            _userContexts.Add(userId, new UserContext(user, userId));
            return _userContexts[userId];
        }
    }
}
