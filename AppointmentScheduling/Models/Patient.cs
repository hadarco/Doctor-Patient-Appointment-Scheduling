
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppointmentScheduling.Models
{
    public class Patient
    {
        [Required(ErrorMessage = "Required field")]
        [RegularExpression("[0-9]+$", ErrorMessage = "Numbers only")]
        [StringLength(9, ErrorMessage = "9 ספרות בדיוק")]
        public string PatientID { get; set; }
        [Required(ErrorMessage = "Required field")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Required field")]
        public string LastName { get; set; }
        [Required(ErrorMessage ="Enter your b.Date")]
        public DateTime BirthDate { get; set; }
        [Required(ErrorMessage ="Enter your email")]
        public string PatientEmail { get; set; }
        [Key]
        [Required(ErrorMessage = "Required field")]
        public string UserName { get; set; }
    }
}