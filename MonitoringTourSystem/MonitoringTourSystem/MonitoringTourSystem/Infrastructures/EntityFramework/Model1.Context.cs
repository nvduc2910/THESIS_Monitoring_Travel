﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class monitoring_tour_v3Entities : DbContext
    {
        public monitoring_tour_v3Entities()
            : base("name=monitoring_tour_v3Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<country> countries { get; set; }
        public virtual DbSet<manager> managers { get; set; }
        public virtual DbSet<message> messages { get; set; }
        public virtual DbSet<place> places { get; set; }
        public virtual DbSet<province> provinces { get; set; }
        public virtual DbSet<tour> tours { get; set; }
        public virtual DbSet<tour_participants> tour_participants { get; set; }
        public virtual DbSet<tour_schedule> tour_schedule { get; set; }
        public virtual DbSet<tourguide> tourguides { get; set; }
        public virtual DbSet<tourist> tourists { get; set; }
        public virtual DbSet<tracking> trackings { get; set; }
        public virtual DbSet<warning> warnings { get; set; }
        public virtual DbSet<warning_receiver> warning_receiver { get; set; }
        public virtual DbSet<area> areas { get; set; }
        public virtual DbSet<notify> notifies { get; set; }
        public virtual DbSet<tourguide_help> tourguide_help { get; set; }
    }
}
