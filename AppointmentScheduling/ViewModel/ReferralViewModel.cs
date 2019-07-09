using AppointmentScheduling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppointmentScheduling.ViewModel
{
    public class ReferralViewModel 
    {
        public Referral Referral { get; set; }
        public List<Referral> Referrals { get; set; }
    }
}
