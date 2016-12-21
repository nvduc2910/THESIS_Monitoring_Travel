using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonitoringTourSystem.RealTimeServer.Model
{
    public class RoomChat
    {
        public string GroupName { get; set; }        
        public List<Connection> UserConnection { get; set; }
    }

    public class Connection
    {
        public string ConectionId { get; set; }
        public string UserId { get; set; }
    }
}