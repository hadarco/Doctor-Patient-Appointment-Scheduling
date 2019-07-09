using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using AppointmentScheduling.Models;

namespace AppointmentScheduling.DAL
{
    public class DoctorDal : DbContext
    {
        public DbSet<Doctor> Users { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Doctor>().ToTable("Doctor");
        }

    }
}