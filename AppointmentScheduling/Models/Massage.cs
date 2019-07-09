using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AppointmentScheduling.Models
{
    public class Massage
    {
        [Key, Column(Order = 0)]
        public string SenderUserName { get; set; }
        [Key, Column(Order = 1)]
        public string ReciverUserName { get; set; }
        [Key, Column(Order = 2)]
        public DateTime date { get; set; }
        public string msg { get; set; }
        public bool Read { get; set; }
    }
}