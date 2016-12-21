using MonitoringTourSystem.RealTimeServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonitoringTourSystem.RealTimeServer.BaseMappingConnection
{
    public class ConnectionMappingGroup
    {
        public readonly  List<RoomChat> _groups = new List<RoomChat>();

        public void Add(string key, string connectionId, string userId)
        {

            lock (_groups)
            {
                for (int i = 0; i < _groups.Count; i++)
                {
                    if(_groups[i].GroupName == key)
                    {
                        _groups[i].UserConnection.Add( new Connection() { ConectionId = connectionId, UserId = userId });
                        return;
                    }
                }
                _groups.Add(new RoomChat() { GroupName = key, UserConnection = new List<Connection>() { new Connection() { ConectionId = connectionId, UserId = userId } } });
            }
        }

        public void Remove(string key, string connectionId, string userId)
        {
            lock(_groups)
            {
                for(int i = 0; i < _groups.Count; i++)
                {
                    if(_groups[i].GroupName == key)
                    {
                        var itemRemove = _groups[i].UserConnection.Remove( new Connection() { ConectionId = connectionId, UserId = userId });
                        return;
                    }
                }
            }
        }
    }
}