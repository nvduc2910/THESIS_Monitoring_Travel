using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonitoringTourSystem.Models
{
    public class Province
    {
        public Dictionary<int, string> ProvinceList = new Dictionary<int, string>();
        public Province()
        {
            ProvinceList.Add(43, "ĐN");
            ProvinceList.Add(76, "VT");
            ProvinceList.Add(49, "LĐ");
        }
    }
}