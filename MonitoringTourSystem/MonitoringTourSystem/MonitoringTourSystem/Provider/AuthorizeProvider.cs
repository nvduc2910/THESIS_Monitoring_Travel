using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonitoringTourSystem.Provider
{
    public class AuthorizeProvider
    {

        public string GenToken()
        {
            return "TOKEN_STRING";
        }

        public bool CheckToken()
        {
            return true;
        }

    }
}