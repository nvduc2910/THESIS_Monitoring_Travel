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
    
    public partial class warning
    {
        public int warning_id { get; set; }
        public string warning_name { get; set; }
        public bool is_group { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public Nullable<double> distance { get; set; }
    }
}
