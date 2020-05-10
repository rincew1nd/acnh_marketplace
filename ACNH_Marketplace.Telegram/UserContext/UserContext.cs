﻿using ACNH_Marketplace.DataBase.Models;
using ACNH_Marketplace.Telegram.Enums;
using System.Collections.Generic;

namespace ACNH_Marketplace.Telegram
{
    public class UserContext
    {
        public int UserId;
        public Dictionary<UserContextEnum, object> _userContext;

        public UserContext(User user, int userId)
        {
            UserId = userId;
            if (user != null)
            {
                _userContext = new Dictionary<UserContextEnum, object>()
                {
                    { UserContextEnum.UserState, UserStateEnum.Default },
                    { UserContextEnum.InGameName, user.InGameName },
                    { UserContextEnum.IslandName, user.IslandName }
                };
            }
            else
            {
                _userContext = new Dictionary<UserContextEnum, object>()
                {
                    { UserContextEnum.UserState, UserStateEnum.Welcome }
                };
            }
        }

        public T GetContext<T>(UserContextEnum ucEnum)
        {
            return (T)_userContext[ucEnum];
        }

        public bool SetContext<T>(UserContextEnum ucEnum, T obj)
        {
            if (_userContext.ContainsKey(ucEnum))
                _userContext[ucEnum] = obj;
            else
                _userContext.Add(ucEnum, obj);
            return true;
        }
    }
}
