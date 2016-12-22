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
using MonitoringTourSystem.Infrastructures;

namespace MonitoringTourSystem
{

    public class HubServer : Hub
    {
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();
        private readonly static ConnectionMappingGroup _groups = new ConnectionMappingGroup();
        protected DbContext _dbContextPool = new DbContext();
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
            string userName = Context.QueryString["USER_NAME"];

            if (!_connections.GetConnections(userId).Contains(Context.ConnectionId))
            {
                _connections.Add(userId, Context.ConnectionId);
                HandleGroup(position, managerId, userId);
            }

            if (position == "MG")
            {
                InformManageOnline(RoomNameDefine.GROUP_NAME_MANAGER + userId);
                UpdateCountUserOnline(RoomNameDefine.GROUP_NAME_MANAGER + userId);
            }
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
                        var numberOfOnline = (_groups._groups[i].UserConnection.Count - 1).ToString();
                        Clients.Group(groupName).updateNumberOfOnline(groupName, numberOfOnline);
                    }
                    else
                    {
                        var numberOfOnline =( _groups._groups[i].UserConnection.Count).ToString();
                        Clients.Group(groupName).updateNumberOfOnline(groupName, numberOfOnline);
                    }
                }
            }
        }

        #region Manager
        public void InitMarkerNewConection(string latitude, string longitude, string receiver, string tourguideId, string tourguideName, string tourId)
        {
            foreach (var connection in _connections.GetConnections(receiver))
            {
                Clients.Client(connection).locationForAddMarker(latitude, longitude, tourguideId, tourguideName,tourId);
            }
        }

        public void RemoveUserDisconnection(string receiver, string senderId, string sernderUserName)
        {
            foreach (var connection in _connections.GetConnections(receiver))
            {
                if (senderId.Contains("TR_"))
                {
                    int senderIdInt = Convert.ToInt32(senderId.Replace("TR_", ""));
                    Clients.Client(connection).removeUserDisconnection(senderIdInt, sernderUserName);
                }
                else
                {
                    Clients.Client(connection).removeUserDisconnection(senderId, sernderUserName);
                }   
            }
        }

        public void UpdatePositionTourGuide(string sender, int tourId, string latitude, string longitude, string receiver)
        {
            foreach (var connection in _connections.GetConnections(receiver))
            {
                if (receiver.Contains("MG"))
                {
                    Clients.Client(connection).receiveLoaction(sender, latitude, longitude);

                    var location = new Location()
                    {
                        lat = Convert.ToDouble(latitude),
                        lng = Convert.ToDouble(longitude),
                    };
                    WriteLocationTracking(sender, location, tourId);

                }
                else if (receiver.Contains("TG"))
                {
                    Clients.Client(connection).receiveLoactionTourist(Convert.ToInt32(sender), latitude, longitude);
                }
            }
        }

        public void SendWarning(Warning obj)
        {
            var id = _dbContextPool.GetContext().warnings.Max(x => x.warning_id);

            obj.WarningId = id;

            for (int i = 0; i < obj.ListTourGuideId.Count; i++)
            {
                foreach (var connection in _connections.GetConnections(obj.ListTourGuideId[i]))
                {
                    Clients.Client(connection).receiveWarning(obj);
                }
            }
        }


        public void InsertNofity( string notify_content, int notify_receiver, string notify_title, string notify_type)
        {
            using (var context = new monitoring_tour_v3Entities())
            {
                var item = new notify()
                {
                    notify_content = notify_content,
                    notify_receiver = notify_receiver,
                    notify_title = notify_title,
                    notify_type = notify_type,
                    time_create = DateTime.Now,
                    status = "New",
                };

                context.notifies.Add(item);
                context.SaveChanges();
            }
        }
        public void SendWarningForUser(Warning obj, int receiverId)
        {

            var id = _dbContextPool.GetContext().warnings.Max(x => x.warning_id);
            obj.WarningId = id;
            var receveriIdStr = "TG_" + receiverId.ToString();
            foreach (var connection in _connections.GetConnections(receveriIdStr))
            {
                Clients.Client(connection).receiveWarning(obj);
            }
        }

        public void ConfirmWarning(int warningId, string warningName, string tourguideId, string sender, string receiver)
        {
            int warningInt = warningId;
            int tourguideIdInt = Convert.ToInt32(tourguideId);

            using (var context = new monitoring_tour_v3Entities())
            {
                var warningResult = (from x in context.warning_receiver
                                     where x.warning_id == warningInt && x.receiver_id == tourguideIdInt
                                     select x).First();

                warningResult.status = "Confirmed";
                context.SaveChanges();
            }
            foreach (var connection in _connections.GetConnections(receiver))
            {
                Clients.Client(connection).confirmWarning(sender, warningName);
            }
        }

        public void NeedHelp(int tourguideId, string latitude, string longitude, string helpContent, string receiver)
        {
            foreach (var connection in _connections.GetConnections(receiver))
            {
                Clients.Client(connection).needHelpTourguide(tourguideId, latitude, longitude, helpContent);
            }
            using (var context = new monitoring_tour_v3Entities())
            {
                var receiverId = Convert.ToInt32(receiver.Replace("MG_", string.Empty));

                var helpRecord = new tourguide_help()
                {
                    sender_id = tourguideId,
                    receiver_id = receiverId,
                    lat = latitude,
                    lng = longitude,
                    help_content = helpContent,
                    status = HelpStatus.Opening.ToString(),
                };
                context.tourguide_help.Add(helpRecord);
                context.SaveChanges();
            }
        }

        // Write Location Tracking to database 
        private static Dictionary<string, Location> lastLocationUpdate = new Dictionary<string, Location>();
        private static Dictionary<string, Location> tempLocationNow = new Dictionary<string, Location>();
        private static Dictionary<string, DateTime?> lastManagerTime = new Dictionary<string, DateTime?>();
        private static Dictionary<string, bool> isWriting = new Dictionary<string, bool>();

        public  void WriteLocationTracking(string sender, Location location, int tourId)
        {
            var senderInt = Convert.ToInt32(sender);

            if (!lastManagerTime.ContainsKey(tourId.ToString()))
            {
                lastManagerTime.Add(tourId.ToString(), null);
            }
            if (!tempLocationNow.ContainsKey(tourId.ToString()))
            {
                tempLocationNow.Add(tourId.ToString(), null);
            }
            if (!lastManagerTime.ContainsKey(tourId.ToString()))
            {
                lastManagerTime.Add(tourId.ToString(), null);
            }
            if (!isWriting.ContainsKey(tourId.ToString()))
            {
                isWriting.Add(tourId.ToString(), false);
            }
            try
            {
                Console.Write("Checking to write");
                bool isNeedWriteLocation = false;

                if (lastManagerTime[tourId.ToString()] == null)
                {
                    isNeedWriteLocation = true;
                }
        
                else
                {
                    TimeSpan ts = DateTime.Now.Subtract((DateTime)lastManagerTime[tourId.ToString()]);
                    var distance = GetDistanceFromLatLonInKm(location.lat, location.lng, lastLocationUpdate[tourId.ToString()].lat, lastLocationUpdate[tourId.ToString()].lng);
                    if (distance > 0.02)
                    {
                        isNeedWriteLocation = true;
                    }
                }
                if (isNeedWriteLocation)
                {
                    Console.Write("Need write location");
                    if (!isWriting[tourId.ToString()])
                    {
                        Console.WriteLine("Insert to database");
                        tempLocationNow[tourId.ToString()] = location;
                        isWriting[tourId.ToString()] = true;

                        // Write to  database

                        using (var context = new monitoring_tour_v3Entities())
                        {
                            var trackingLocation = new tracking()
                            {
                                tour_id = tourId,
                                latitude = location.lat,
                                longitude = location.lng,
                                time = DateTime.Now,
                            };

                            var locationTourguide = (from x in context.tourguides
                                                     where x.tourguide_id == senderInt
                                                     select x).First();
                            locationTourguide.latitude = location.lat;
                            locationTourguide.longitude = location.lng;

                            context.trackings.Add(trackingLocation);
                            context.SaveChanges();
                        }

                        lastManagerTime[tourId.ToString()] = DateTime.Now;
                        lastLocationUpdate = tempLocationNow;
                        isWriting[tourId.ToString()] = false;

                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        double GetDistanceFromLatLonInKm(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371d; // Radius of the earth in km
            var dLat = Deg2Rad(lat2 - lat1);  // deg2rad below
            var dLon = Deg2Rad(lon2 - lon1);
            var a =
              Math.Sin(dLat / 2d) * Math.Sin(dLat / 2d) +
              Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) *
              Math.Sin(dLon / 2d) * Math.Sin(dLon / 2d);
            var c = 2d * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1d - a));
            var d = R * c; // Distance in km
            return d;
        }
        double Deg2Rad(double deg)
        {
            return deg * (Math.PI / 180d);
        }

        #endregion

        #region TourGuide

        // Handle For Tourguid from Tourist

        public void InitTouristConnection(double latitude, double longitude, string receiver, string touristName, int touristId)
        {
            foreach (var connection in _connections.GetConnections(receiver))
            {
                Clients.Client(connection).initTouristConnected(latitude, longitude, touristName, touristId);
            }
        }
        public void WarningForTourist(int sender, double latitude, double longitude, string warningContent, string warningTitle)
        {
            var groupName = RoomNameDefine.GROUP_NAME_TOURGUIDE + "TG_" + sender;
            Clients.Group(groupName).receiveTourguideWarning(sender, latitude, longitude, warningContent, warningTitle);

            using (var context = new monitoring_tour_v3Entities())
            {
                //insert warning tourguide for tourist
                var itemWarning = new warning_tourist()
                {
                    warning_content = warningContent,
                    lat = latitude,
                    lng = longitude,
                    status = StatusWarning.Opening.ToString(),
                    warning_title = warningTitle,
                };

                context.warning_tourist.Add(itemWarning);
                context.SaveChanges();


                //insert warning recevier
                var listConnectIdTourist = new List<Connection>();
                for (int i = 0; i < _groups._groups.Count; i++)
                {
                    if (_groups._groups[i].GroupName == RoomNameDefine.GROUP_NAME_TOURGUIDE + "TG_" + sender)
                    {
                        listConnectIdTourist = _groups._groups[i].UserConnection;
                        break;
                    }
                }

                int warningId = context.warning_tourist.Max(p => p.warning_id);

                for (int j = 0; j < listConnectIdTourist.Count; j++)
                {
                    if (listConnectIdTourist[j].UserId.Contains("TR_"))
                    {
                        var itemWarningTouristReceiver = new warning_tourist_receiver()
                        {
                            sender_id = sender,
                            receiver_id = Convert.ToInt32(listConnectIdTourist[j].UserId.Replace("TR_", "")),
                            warning_id = warningId,
                            status = StatusWarning.Opening.ToString()
                        };
                        context.warning_tourist_receiver.Add(itemWarningTouristReceiver);
                    }
                }
                context.SaveChanges();
            }
        }

 
        public void TouristNeedHelp(int touristId, double latitude, double longitude, string helpContent, string receiver)
        {
            foreach (var connection in _connections.GetConnections(receiver))
            {
                Clients.Client(connection).receiveTouristHelp(touristId, latitude, longitude, helpContent);
            }
            //using (var context = new monitoring_tour_v3Entities())
            //{

            //    var receiverId = Convert.ToInt32(receiver.Replace("MG_", string.Empty));

            //    //var helpRecord = new tourguide_help()
            //    //{
            //    //    sender_id = touristId,
            //    //    receiver_id = receiverId,
            //    //    lat = latitude,
            //    //    lng = longitude,
            //    //    help_content = helpContent,
            //    //    status = HelpStatus.Opening.ToString(),
            //    //};
            //    //context.tourguide_help.Add(helpRecord);
            //    //context.SaveChanges();
            //}
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
               //_groups.Add(RoomNameDefine.GROUP_NAME_MANAGER + userId, Context.ConnectionId);

                UpdateCountUserOnline(RoomNameDefine.GROUP_NAME_MANAGER + userId);
            }
            else if (userPosition == "TG")
            {
                Groups.Add(Context.ConnectionId, RoomNameDefine.GROUP_NAME_MANAGER + managerId);
                _groups.Add(RoomNameDefine.GROUP_NAME_MANAGER + managerId, Context.ConnectionId, userId);
                UpdateCountUserOnline(RoomNameDefine.GROUP_NAME_MANAGER + managerId);

                Groups.Add(Context.ConnectionId, RoomNameDefine.GROUP_NAME_TOURGUIDE + userId);
                _groups.Add(RoomNameDefine.GROUP_NAME_TOURGUIDE + userId, Context.ConnectionId, userId);
                UpdateCountUserOnline(RoomNameDefine.GROUP_NAME_TOURGUIDE + userId);

                _connections.Add(userId, Context.ConnectionId);
            }
            else if (userPosition == "TR")
            {
                Groups.Add(Context.ConnectionId, RoomNameDefine.GROUP_NAME_TOURGUIDE + managerId);
                _connections.Add(userId, Context.ConnectionId);
                _groups.Add(RoomNameDefine.GROUP_NAME_TOURGUIDE + managerId, Context.ConnectionId, userId);
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
                _groups.Remove(RoomNameDefine.GROUP_NAME_MANAGER + userId, Context.ConnectionId, userId);
                UpdateCountUserOnline(RoomNameDefine.GROUP_NAME_MANAGER + userId);
            }
            else if (userPosition == "TG")
            {
                //Groups.Add(Context.ConnectionId, RoomNameDefine.GROUP_NAME_MANAGER + managerId);
                _groups.Remove(RoomNameDefine.GROUP_NAME_MANAGER + managerId, Context.ConnectionId, userId);
                UpdateCountUserOnline(RoomNameDefine.GROUP_NAME_MANAGER + managerId);

                //Groups.Add(Context.ConnectionId, RoomNameDefine.GROUP_NAME_TOURGUIDE + userId);
                _groups.Remove(RoomNameDefine.GROUP_NAME_TOURGUIDE + userId, Context.ConnectionId, userId);
                UpdateCountUserOnline(RoomNameDefine.GROUP_NAME_TOURGUIDE + userId);

                _connections.Remove(userId, Context.ConnectionId);
            }
            else if (userPosition == "TR")
            {
               // Groups.Add(Context.ConnectionId, RoomNameDefine.GROUP_NAME_TOURGUIDE + managerId);
                _connections.Remove(userId, Context.ConnectionId);
                _groups.Remove(RoomNameDefine.GROUP_NAME_TOURGUIDE + managerId, Context.ConnectionId, userId);
                UpdateCountUserOnline(RoomNameDefine.GROUP_NAME_TOURGUIDE + managerId);
            }
        }
        #endregion
    }

}
