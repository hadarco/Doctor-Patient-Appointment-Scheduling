using AppointmentScheduling.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AppointmentScheduling.DAL
{
    public class AppointmentDal:DbContext
    {
        public DbSet<Appointment> Appointments { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Appointment>().ToTable("Appointment");
        }
    }
}