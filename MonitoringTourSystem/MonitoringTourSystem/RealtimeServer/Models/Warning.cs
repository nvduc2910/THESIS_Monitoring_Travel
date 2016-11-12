using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealtimeServer.Models
{
    public class Warning
    {
        public string WarningName { get; set; }
        public string Categories { get; set; }
        public string Distance { get; set; }
        public List<User> Recevier { get; set; }
        public string Description { get; set; }
        public double Laitude { get; set; }
        public double Longitude { get; set; }
    }
}