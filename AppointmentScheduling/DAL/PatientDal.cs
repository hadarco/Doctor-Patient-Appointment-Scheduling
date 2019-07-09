using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using AppointmentScheduling.Models;

namespace AppointmentScheduling.DAL
{
    public class PatientDal : DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Patient>().ToTable("Patient");
        }

    }
}