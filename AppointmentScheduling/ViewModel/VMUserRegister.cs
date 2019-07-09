using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using AppointmentScheduling.DAL;
using AppointmentScheduling.Models;

namespace AppointmentScheduling.viewModel
{
     public class VMUserRegister
    {
        [Key]
        [Required(ErrorMessage = "Required field")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "שדה חובה")]
        [RegularExpression("[a-zA-Z0-9]+$", ErrorMessage = "אותיות באנגלית, לפחות אחת גדולה ולפחות אחת קטנה, לפחות ספרה אחת")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "לפחות 8 תווים לכל היותר 20")]
        public string Password { get; set; }     
        [Required(ErrorMessage = "שדה חובה")]
        [RegularExpression("[a-zA-Z0-9]+$", ErrorMessage = "אותיות באנגלית, לפחות אחת גדולה ולפחות אחת קטנה, לפחות ספרה אחת")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "לפחות 8 תווים לכל היותר 20")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "הזן את אותה סיסמה")]
        public string ConfirmPassword { get; set; }
        public Patient PatientDetails { get; set; }
        public User NewUser { get; set; }
        
        public VMUserRegister()
        {
            PatientDetails = new Patient();
            NewUser = new User();   
            }
}
}

