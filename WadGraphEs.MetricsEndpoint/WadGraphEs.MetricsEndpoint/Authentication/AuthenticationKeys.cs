using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WadGraphEs.MetricsEndpoint.Factories;
using WadGraphEs.MetricsEndpoint.Lib;

namespace WadGraphEs.MetricsEndpoint.Authentication{
    public class AuthenticationKeys {
        public static bool CheckKey(string key) {
            return MetricsConfigEndpointConfigurationFactory.New().AuthenticateKey(key);
        }
    }
}