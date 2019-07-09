using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppointmentScheduling.Models
{
    public class UserLogin
    {
        [Key]
        [Required (ErrorMessage = "Required field")]
        public string UserName { get; set; }
        [Required (ErrorMessage = "Required field")]
        public string Password { get; set; }
    }
}