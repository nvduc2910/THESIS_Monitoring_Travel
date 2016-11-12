using Microsoft.AspNet.SignalR;
using RealtimeServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealtimeServer
{
    public class WarningHub : Hub
    {
        //just added to create dummy user Id :)
        static int userId;

        private static List<Models.PlaneSeatsArrangement> allSeats = new List<Models.PlaneSeatsArrangement>();

        public void CreateUser()
        {
            userId++;
            //Clients.All.createUser(userId);
        }
        
        public void SendWarning(Warning obj)
        {   
            string data = "sendSuccessful";
            Clients.Others.receiverWarning(data);
        }
        public void UpdateLocation(Location location)
        {
            Clients.Others.selectSeat(location);
        }

    }
}