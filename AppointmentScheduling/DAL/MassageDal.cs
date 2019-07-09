using AppointmentScheduling.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AppointmentScheduling.DAL
{
    public class MassageDal : DbContext
    {
        public DbSet<Massage> Massages { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Massage>().ToTable("Massage");
        }
    }
}