using Microsoft.AspNet.SignalR;
using MonitoringTourSystem.RealTimeServer.BaseMappingConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using MonitoringTourSystem.Infrastructures.EntityFramework;
using System.Web.Security;
using MonitoringTourSystem.RealTimeServer.Model;

namespace MonitoringTourSystem
{

    public class HubServer : Hub
    {
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();
        private readonly static ConnectionMappingGroup _groups = new ConnectionMappingGroup();

        public override Task OnConnected()
        {

            string position = Context.QueryString["USER_POSITION"];
            string managerId = Context.QueryString["MANAGER_ID"];
            string userId = Context.QueryString["USER_ID"];
            if (userId.Contains("TG"))
            {
                PushAlert("MG_1", userId + "da ket noi vao he thong");
            }
            HandleGroup(position, managerId, userId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string position = Context.QueryString["USER_POSITION"];
            string managerId = Context.QueryString["MANAGER_ID"];
            string userId = Context.QueryString["USER_ID"];

            HandleRemoveGroup(position, managerId, userId);       
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            string position = Context.QueryString["USER_POSITION"];
            string managerId = Context.QueryString["MANAGER_ID"];
            string userId = Context.QueryString["USER_ID"];
            ////string name = Context.QueryString["MANAGER_ID"];
            //if (name != null)
            //{
            //    if (!_connections.GetConnections(name).Contains(Context.ConnectionId))
            //    {
            //        _connections.Add(name, Context.ConnectionId);
            //        _groups.Add(RoomNameDefine.GROUP_NAME_MANAGER + name, Context.ConnectionId);

            //        UpdateCountUserOnline(RoomNameDefine.GROUP_NAME_MANAGER + name);
            //    }
            //}


            return base.OnReconnected();
        }

        public void SendMessageInGroup(string message)
        {
            //var groupId = SetGroupId();
            //var groupName = RoomNameDefine.GROUP_NAME_MANAGER + groupId;
            //Clients.Group(groupName).sendMessager(message);
        }

        public void SendMessageTo(List<string> who, string message)
        {
            for (int i = 0; i < who.Count; i++)
            {
                foreach (var connection in _connections.GetConnections(who[i]))
                {
                    Clients.Client(connection).sendMessager(message);
                }
            }
        }

        // Update Number of user online
        public void UpdateCountUserOnline(string groupName)
        {
            for (int i = 0; i < _groups._groups.Count; i++)
            {
                if (_groups._groups[i].GroupName == groupName)
                {
                    Clients.Group(groupName).updateNumberOfOnline(_groups._groups[i].ConnectionId.Count);
                }
            }
        }

        // send warning
        public void MakeMarkerNewConection( string latitude, string longitude)
        {

            foreach (var connection in _connections.GetConnections("MG_1"))
            {
                Clients.Client(connection).locationForAddMarker(latitude, longitude);
            }

        }
        public void UpdatePositionTourGuide(string user, string latitude, string longitude)
        {
            foreach (var connection in _connections.GetConnections("MG_1"))
            {
                Clients.Client(connection).receiveLoaction(user, latitude, longitude);
            }
        }

        public void AddMarker(object marker)
        {
            // add marker user when a new user connect
        }

        public void Remove(object marker)
        {
            // remove marker user when a new user disconnect
        }

        public void PushAlert(string userIdManager, string message)
        {
            
            foreach (var connection in _connections.GetConnections(userIdManager))
            {
                Clients.Client(connection).pushAlert(message);
            }
        }



        #region Check position and add group

        private void HandleGroup(string userPosition, string managerId, string userId)
        {
            if (userPosition == "MG")
            {
                Groups.Add(Context.ConnectionId, RoomNameDefine.GROUP_NAME_MANAGER + userId);
                _connections.Add(userId, Context.ConnectionId);
                if (managerId == userId)
                { }
                else
                {
                    _groups.Add(RoomNameDefine.GROUP_NAME_MANAGER + userId, Context.ConnectionId);
                }

                UpdateCountUserOnline(RoomNameDefine.GROUP_NAME_MANAGER + userId);
            }
            else if (userPosition == "TG")
            {
                Groups.Add(Context.ConnectionId, RoomNameDefine.GROUP_NAME_MANAGER + managerId);
                _groups.Add(RoomNameDefine.GROUP_NAME_MANAGER + managerId, Context.ConnectionId);
                UpdateCountUserOnline(RoomNameDefine.GROUP_NAME_MANAGER + managerId);

                Groups.Add(Context.ConnectionId, RoomNameDefine.GROUP_NAME_TOURGUIDE + userId);
                _groups.Add(RoomNameDefine.GROUP_NAME_TOURGUIDE + userId, Context.ConnectionId);
                UpdateCountUserOnline(RoomNameDefine.GROUP_NAME_TOURGUIDE + userId);

                _connections.Add(userId, Context.ConnectionId);
            }
            else if (userPosition == "TR")
            {
                Groups.Add(Context.ConnectionId, RoomNameDefine.GROUP_NAME_TOURGUIDE + managerId);
                _connections.Add(userId, Context.ConnectionId);
                _groups.Add(RoomNameDefine.GROUP_NAME_TOURGUIDE + managerId, Context.ConnectionId);
                UpdateCountUserOnline(RoomNameDefine.GROUP_NAME_TOURGUIDE + managerId);
            }
        }

        #endregion

        #region Check position and remove group

        private void HandleRemoveGroup(string userPosition, string managerId, string userId)
        {
            if (userPosition == "MG")
            {
               // Groups.Add(Context.ConnectionId, RoomNameDefine.GROUP_NAME_MANAGER + userId);
                _connections.Remove(userId, Context.ConnectionId);
                if (managerId == userId)
                { }
                else
                {
                    _groups.Remove(RoomNameDefine.GROUP_NAME_MANAGER + userId, Context.ConnectionId);
                }

                UpdateCountUserOnline(RoomNameDefine.GROUP_NAME_MANAGER + userId);
            }
            else if (userPosition == "TG")
            {
                //Groups.Add(Context.ConnectionId, RoomNameDefine.GROUP_NAME_MANAGER + managerId);
                _groups.Remove(RoomNameDefine.GROUP_NAME_MANAGER + managerId, Context.ConnectionId);
                UpdateCountUserOnline(RoomNameDefine.GROUP_NAME_MANAGER + managerId);

                //Groups.Add(Context.ConnectionId, RoomNameDefine.GROUP_NAME_TOURGUIDE + userId);
                _groups.Remove(RoomNameDefine.GROUP_NAME_TOURGUIDE + userId, Context.ConnectionId);
                UpdateCountUserOnline(RoomNameDefine.GROUP_NAME_TOURGUIDE + userId);

                _connections.Remove(userId, Context.ConnectionId);
            }
            else if (userPosition == "TR")
            {
               // Groups.Add(Context.ConnectionId, RoomNameDefine.GROUP_NAME_TOURGUIDE + managerId);
                _connections.Remove(userId, Context.ConnectionId);
                _groups.Remove(RoomNameDefine.GROUP_NAME_TOURGUIDE + managerId, Context.ConnectionId);
                UpdateCountUserOnline(RoomNameDefine.GROUP_NAME_TOURGUIDE + managerId);
            }
        }

        #endregion
    }

}