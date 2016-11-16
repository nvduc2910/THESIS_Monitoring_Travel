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
using Newtonsoft.Json;
using MonitoringTourSystem.Models;

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
            string userName = Context.QueryString["USER_NAME"];

            HandleGroup(position, managerId, userId);
            if (position == "MG")
            {
                InformManageOnline(RoomNameDefine.GROUP_NAME_MANAGER + userId);
                UpdateCountUserOnline(RoomNameDefine.GROUP_NAME_MANAGER + userId);
            }
           
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string position = Context.QueryString["USER_POSITION"];
            string managerId = Context.QueryString["MANAGER_ID"];
            string userId = Context.QueryString["USER_ID"];
            string userName = Context.QueryString["USER_NAME"];

            HandleRemoveGroup(position, managerId, userId);
            RemoveUserDisconnection(managerId, userId, userName);

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

        public void SendMessageTo(List<string> who, object message)
        {
            for (int i = 0; i < who.Count; i++)
            {
                foreach (var connection in _connections.GetConnections(who[i]))
                {
                    Clients.Client(connection).sendMessager(message);
                }
            }
        }

        public void InformManageOnline(string groupName)
        {
            Clients.Group(groupName).managerOnline("Online");
        }

        // Update Number of user online
        public void UpdateCountUserOnline(string groupName)
        {
            
            for (int i = 0; i < _groups._groups.Count; i++)
            {
                if (_groups._groups[i].GroupName == groupName)
                {

                    if (groupName.Contains(RoomNameDefine.GROUP_NAME_TOURGUIDE))
                    {
                        var numberOfOnline = (_groups._groups[i].ConnectionId.Count - 1).ToString();
                        Clients.Group(groupName).updateNumberOfOnline(groupName, numberOfOnline);
                    }
                    else
                    {
                        var numberOfOnline = _groups._groups[i].ConnectionId.Count.ToString();
                        Clients.Group(groupName).updateNumberOfOnline(groupName, numberOfOnline);
                    }
                }
            }
        }

        #region Manager
        // Handle For Manager From Touguide
        //public void InitMarkerNewConection(string latitude, string longitude, string receiver, tourguide tourguide, tour tour)
        //{

        //    var jsonTourguideInfo = JsonConvert.SerializeObject(tourguide);
        //    var jsonTourInfo = JsonConvert.SerializeObject(tour);

        //    foreach (var connection in _connections.GetConnections(receiver))
        //    {
        //        Clients.Client(connection).locationForAddMarker(latitude, longitude, jsonTourguideInfo, jsonTourInfo);
        //    }
        //}


        public void InitMarkerNewConection(string latitude, string longitude, string receiver, tourguide tourguide, tour tour)
        {

            var jsonTourguideInfo = JsonConvert.SerializeObject(tourguide);
            var jsonTourInfo = JsonConvert.SerializeObject(tour);

            foreach (var connection in _connections.GetConnections(receiver))
            {
                Clients.Client(connection).locationForAddMarker(latitude, longitude, jsonTourguideInfo, jsonTourInfo);
            }
        }

        public void RemoveUserDisconnection(string receiver, string senderId, string sernderUserName)
        {
            foreach (var connection in _connections.GetConnections(receiver))
            {
                Clients.Client(connection).removeUserDisconnection(senderId, sernderUserName);
            }
        }

        public void UpdatePositionTourGuide(string sender, string latitude, string longitude, string receiver)
        {
            foreach (var connection in _connections.GetConnections(receiver))
            {
                Clients.Client(connection).receiveLoaction(sender, latitude, longitude);
            }
        }


        public void SendWarning(Warning obj)
        {
            for (int i = 0; i < obj.ListTourGuideId.Count; i++)
            {
                foreach (var connection in _connections.GetConnections(obj.ListTourGuideId[i]))
                {
                    Clients.Client(connection).receiverWarning(obj);
                }
            }
        }

        #endregion


        #region TourGuide

        // Handle For Tourguid from Tourist

        public void InitTouristConnection(string latitude, string longitude, string receiver, string touristName)
        {
            foreach (var connection in _connections.GetConnections(receiver))
            {
                Clients.Client(connection).initTouristConnected(latitude, longitude, touristName);
            }
        }


        #endregion






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