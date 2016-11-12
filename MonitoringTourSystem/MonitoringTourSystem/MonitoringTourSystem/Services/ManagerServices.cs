using MonitoringTourSystem.Infrastructures;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace MonitoringTourSystem.Services
{
    public class ManagerServices : Controller
    {
        protected DbContext _dbContextPool = new DbContext();
        public  int GetUserID(string username)
        {
            return Convert.ToInt32(username);
        }
    }
}