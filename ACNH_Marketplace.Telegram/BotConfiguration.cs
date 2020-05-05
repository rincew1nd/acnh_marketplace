﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ACNH_Marketplace.Telegram
{
    public class BotConfiguration
    {
        public string Token { get; set; }

        public ProxyData Proxy { get; set; }

        public class ProxyData
        {
            public string Address { get; set; }
            public int Port { get; set; }
            public string User { get; set; }
            public string Password { get; set; }
        }
    }
}
