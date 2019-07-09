using AppointmentScheduling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace AppointmentScheduling.ViewModel
{
    public class DoctorViewModel
    {
        public Doctor Doctor { get; set; }
        public List<Doctor> Doctors { get; set; }
    }
}
