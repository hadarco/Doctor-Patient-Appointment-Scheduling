using AppointmentScheduling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace AppointmentScheduling.ViewModel
{
    public class PatientViewModel
    {
        public Patient Patient { get; set; }
        public List<Patient> Patients { get; set; }
    }
}