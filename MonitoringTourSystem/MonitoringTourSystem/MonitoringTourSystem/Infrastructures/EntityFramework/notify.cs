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
    
    public partial class notify
    {
        public int notify_id { get; set; }
        public string notify_content { get; set; }
        public int notify_receiver { get; set; }
        public string status { get; set; }
        public System.DateTime time_create { get; set; }
        public string notify_title { get; set; }
        public string notify_type { get; set; }
    }
}