using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AppointmentScheduling.Models
{
    public class Appointment
    {
        
        public string PatientUserName { get; set; }
        [Key, Column(Order = 0)]
        [Required]
        public DateTime Date { get; set; }
        [Key, Column(Order = 1)]
        [Required]
        public string DoctorName { get; set; }
    }
}