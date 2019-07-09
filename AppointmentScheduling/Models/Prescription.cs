using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace AppointmentScheduling.Models
{
    public class Prescription
    {
       
        [Required]
        [RegularExpression("^[1-9]{5}$", ErrorMessage = "Doctor License must be 5 digits (digits 1-9)")]
        public string DoctorLicense { get; set; }

        [Key]
        [ForeignKey("Patient")]
        [Required]
        [RegularExpression("^[A-Za-z ]*$", ErrorMessage = "Must use letters only")]
        public string PatientID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime IssueDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ExpDate { get; set; }

        [Key]
        [Required(ErrorMessage = "Medication is required")]
        public string Medication { get; set; }

    }
}
