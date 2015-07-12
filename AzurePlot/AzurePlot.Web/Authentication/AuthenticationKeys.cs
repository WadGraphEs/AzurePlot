using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using AzurePlot.Lib;
using AzurePlot.Web.Setup;

namespace AzurePlot.Web.Authentication{
    public class AuthenticationKeys {
        public static bool CheckKey(string key) {
			return APIEndpoint.AuthenticateKey(key);
            
        }
    }
}