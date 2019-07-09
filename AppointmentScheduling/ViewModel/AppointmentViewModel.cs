using AppointmentScheduling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace AppointmentScheduling.ViewModel
{
    public class AppointmentViewModel
    {
        public Appointment Appointment { get; set; }
        public List<Appointment> Appointments { get; set; }
    }
}