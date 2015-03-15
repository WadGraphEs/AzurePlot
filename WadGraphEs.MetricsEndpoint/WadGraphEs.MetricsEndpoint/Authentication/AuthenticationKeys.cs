using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WadGraphEs.MetricsEndpoint.Lib;
using WadGraphEs.MetricsEndpoint.Setup;

namespace WadGraphEs.MetricsEndpoint.Authentication{
    public class AuthenticationKeys {
        public static bool CheckKey(string key) {
			return APIEndpoint.AuthenticateKey(key);
            
        }
    }
}