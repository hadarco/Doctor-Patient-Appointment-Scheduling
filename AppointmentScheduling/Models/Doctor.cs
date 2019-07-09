using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppointmentScheduling.Models
{
    public class Doctor {
        
        [Required]
        [RegularExpression("^[1-9]{5}$", ErrorMessage = "Doctor License must be 5 digits (digits 1-9)")]
        public string DoctorLicenese { get; set; }

        [Required]
        [RegularExpression("^[A-Za-z ]*$", ErrorMessage = "Must use letters only")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression("^[A-Za-z ]*$", ErrorMessage = "Must use letters only")]
        public string LastName { get; set; }

        [Required]
        [RegularExpression("^[0-9]{9}$", ErrorMessage = "Doctor Id must be 9 digits (digits 0-9)")]
        public string DoctorID { get; set; }

        [Required(ErrorMessage = "The Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string DoctorEmail { get; set; }

        [Required(ErrorMessage = "The Doctor Specialization is required")]
        public string DoctorSpecialization { get; set; }
        [Key]
        [Required]
        public string UserName { get; set; }

    }
}