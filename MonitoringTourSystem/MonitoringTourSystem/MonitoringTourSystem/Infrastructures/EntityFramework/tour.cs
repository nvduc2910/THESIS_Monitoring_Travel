//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MonitoringTourSystem.Infrastructures.EntityFramework
{
    using System;
    using System.Collections.Generic;
    
    public partial class tour
    {
        public int tour_id { get; set; }
        public string tour_code { get; set; }
        public int manager_id { get; set; }
        public int tourguide_id { get; set; }
        public string tour_name { get; set; }
        public System.DateTime departure_date { get; set; }
        public System.DateTime return_date { get; set; }
        public int tourist_quantity { get; set; }
        public string status { get; set; }
        public string description { get; set; }
        public int day { get; set; }
        public string cover_photo { get; set; }
        public int is_foreign_tour { get; set; }

        public List<tour_schedule> ListTourSchedule { get; set; }

    }
}
