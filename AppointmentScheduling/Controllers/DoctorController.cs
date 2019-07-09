using AppointmentScheduling.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using AppointmentScheduling.Models;
using AppointmentScheduling.ViewModel;
using AppointmentScheduling.Cryptography;

namespace AppointmentScheduling.Controllers
{
    public class DoctorController : Controller
    {
        private DES des = new DES { };
        private AppointmentDal appDal = new AppointmentDal();
        private DoctorDal docDal = new DoctorDal();
        private bool Authorize()
        {
            if (Session["CurrentUser"] == null)
                return false;
            User curr = (User)Session["CurrentUser"];
            Doctor doc = docDal.Users.FirstOrDefault<Doctor>(x => x.UserName == curr.UserName);
            return doc != null;
        }
        public ActionResult DoctorPage()
        {
            if (!Authorize())
                return RedirectToAction("RedirectByUser", "Home");
            return View();
        }
        public ActionResult AddNewAppointments()
        {
            if (!Authorize())
                return RedirectToAction("RedirectByUser", "Home");
            return View(new Appointment());
        }
        public ActionResult AddNewAppointmentSubmit(Appointment app)
        {
            if (!Authorize())
                return RedirectToAction("RedirectByUser", "Home");
            User current = (User)Session["CurrentUser"];  
            DateTime temp = new DateTime(app.Date.Year, app.Date.Month, app.Date.Day,Convert.ToInt32( Request.Form["hour"]), Convert.ToInt32(Request.Form["minu"]), 00);
            app.Date = temp;
            if (app.Date < DateTime.Now)
            {
                ViewBag.appexist = "לא ניתן להוסיף תורים לזמן שעבר";
                return View("AddNewAppointments");
            }
            app.DoctorName = docDal.Users.FirstOrDefault<Doctor>(x => x.UserName == current.UserName).FirstName;
            if(appDal.Appointments.FirstOrDefault<Appointment>(x=>x.Date==app.Date && app.DoctorName == x.DoctorName) != null)
            {
                ViewBag.appexist = "תור קיים";
                return View("AddNewAppointments");
            }
            appDal.Appointments.Add(app);
            appDal.SaveChanges();
            return View("DoctorPage");
        }
        public ActionResult YourAppointments()
        {
            if (!Authorize())
                return RedirectToAction("RedirectByUser", "Home");
            User current = (User)Session["CurrentUser"];
            AppointmentViewModel appVM = new AppointmentViewModel();
            string DoctorName = docDal.Users.FirstOrDefault<Doctor>(x => x.UserName == current.UserName).FirstName;
            DateTime d1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime d2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day+1);
            appVM.Appointments = (from app in appDal.Appointments
                                  where app.DoctorName == DoctorName && app.PatientUserName!=null && app.Date<d2 && app.Date>d1 
                                  select app).ToList<Appointment>();
            for (int i = 0; i < appVM.Appointments.Count; i++)
                appVM.Appointments[i].PatientUserName = des.Decrypt(appVM.Appointments[i].PatientUserName, "Galit@19");
            return View(appVM);
        }



        public ActionResult GetUsersByJson()
        {
            if (!Authorize())
                return RedirectToAction("RedirectByUser", "Home");
            User currentUser = (User)Session["CurrentUser"];
            UserDal usrDal = new UserDal();
            List<string> users = (from usr in usrDal.Users
                                  where usr.UserName!= currentUser.UserName
                                  select usr.UserName).ToList<string>();
            for (int i = 0; i < users.Count; i++)
            {
                users[i] = des.Decrypt(users[i], "Galit@19");
            }
            Thread.Sleep(1000);
            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MassagePage()
        {
            if (!Authorize())
                return RedirectToAction("RedirectByUser", "Home");
            return View();
        }
        public ActionResult NewMessage()
        {
            if (!Authorize())
                return RedirectToAction("RedirectByUser", "Home");
            return View(new Massage());
        }
        [HttpPost]
        public ActionResult SendMessage()
        {
            if (!Authorize())
                return RedirectToAction("RedirectByUser", "Home");
            User CurrentUser = (User)Session["CurrentUser"];
            DateTime dateTime = DateTime.Now;
            dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
            Massage msg = new Massage
            {
                Read = false,
                date = dateTime,
                SenderUserName = CurrentUser.UserName,
                ReciverUserName = des.Encrypt(Request.Form["DoctorCombo"], "Galit@19"),
                msg = Request.Form["msg"]
            };
            TryValidateModel(msg);
            if (ModelState.IsValid)
            {
                MassageDal msgDal = new MassageDal();
                msgDal.Massages.Add(msg);
                msgDal.SaveChanges();
            }
            return View("MassagePage");
        }
        public ActionResult ReciverMessages()
        {
            if (!Authorize())
                return RedirectToAction("RedirectByUser", "Home");
            User CurrentUser = (User)Session["CurrentUser"];
            MassageDal msgDal = new MassageDal();
            VMMassages VMm = new VMMassages
            {
                Massages = (from msg in msgDal.Massages
                            where msg.ReciverUserName == CurrentUser.UserName
                            select msg).ToList<Massage>()
            };
            for (int i = 0; i < VMm.Massages.Count; i++)
                VMm.Massages[i].SenderUserName = des.Decrypt(VMm.Massages[i].SenderUserName,"Galit@19");
            return View(VMm);
        }

        public ActionResult ReadMassage(string sender, DateTime date)
        {
            if (!Authorize())
                return RedirectToAction("RedirectByUser", "Home");
            User CurrentUser = (User)Session["CurrentUser"];
            MassageDal msgDal = new MassageDal();
            string encryptedsender = des.Encrypt(sender, "Galit@19");
            //Massage m = msgDal.Massages.FirstOrDefault<Massage>(x => x.ReciverUserName == CurrentUser.UserName && x.SenderUserName == encryptedsender);
            Massage m = msgDal.Massages.FirstOrDefault<Massage>(x => x.ReciverUserName == CurrentUser.UserName && x.SenderUserName == encryptedsender && x.date == date);
            m.Read = true;
            msgDal.SaveChanges();
            return RedirectToAction("ReciverMessages");
        }

        public ActionResult ShowDetails()
        {
            if (!Authorize())
                return RedirectToAction("RedirectByUser", "Home");
            return View();
        }
        public ActionResult ChangePass()
        {
            if (!Authorize())
                return RedirectToAction("RedirectByUser", "Home");

            return View(new ChangePassword());
        }
        [HttpPost]
        public ActionResult ChangePassSubmit(ChangePassword pass)
        {
            if (!Authorize())
                return RedirectToAction("RedirectByUser", "Home");

            User currentUser = (User)Session["CurrentUser"];
            TryValidateModel(pass);
            if (ModelState.IsValid)
            {
                if (pass.oldPass != des.Decrypt(currentUser.Password, "Galit@19"))
                {
                    ViewBag.pass = "Old password doesn't match! Password hasn't changed";
                    return View("ChangePass");
                }
                UserDal usrDal = new UserDal();
                currentUser = usrDal.Users.FirstOrDefault<User>(x => x.UserName == currentUser.UserName);
                currentUser.Password = des.Encrypt(pass.newPass, "Galit@19");
                usrDal.SaveChanges();
                ViewBag.pass = "Password has changed";
                return View("ShowDetails");
            }
            return View("ChangePass");
        }
    }
}


