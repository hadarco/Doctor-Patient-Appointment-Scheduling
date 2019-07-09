using AppointmentScheduling.DAL;
using AppointmentScheduling.Models;
using AppointmentScheduling.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using AppointmentScheduling.Cryptography;

namespace AppointmentScheduling.Controllers
{
    public class PatientController : Controller
    {
        private DES des = new DES { };
        private bool Authorize()
        {
            if (Session["CurrentUser"] == null)
                return false;
            return true;
        }
        public ActionResult PatientPage()
        {
            if (!Authorize())
                return RedirectToAction("RedirectByUser", "Home");
            return View();
        }
        public ActionResult AppointmentScheduling()
        {
            if (!Authorize())
                return RedirectToAction("RedirectByUser", "Home");
            AppointmentViewModel AppVM = new AppointmentViewModel();
            AppointmentDal appDal = new AppointmentDal();
            AppVM.Appointments = (from app in appDal.Appointments
                                  where app.PatientUserName == null && DateTime.Compare(DateTime.Now, app.Date) < 0
                                  select app).ToList<Appointment>();

            return View(AppVM);
        }
        public ActionResult GetAppointmentsVacantByJson()
        {
            if (!Authorize())
                return RedirectToAction("RedirectByUser", "Home");
            AppointmentDal appDal = new AppointmentDal();
            List<Appointment> appointments = (from app in appDal.Appointments
                                              where app.PatientUserName == null && DateTime.Compare(DateTime.Now, app.Date) < 0
                                              select app).ToList<Appointment>();
            Thread.Sleep(1000);
            return Json(appointments, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChooseAppointment(string DoctorName, DateTime date)
        {
            if (!Authorize())
                return RedirectToAction("RedirectByUser", "Home");
            Appointment chosen = new Appointment { DoctorName = DoctorName, Date = date };
            PatientDal pdal = new PatientDal();
            User currentUser = (User)Session["CurrentUser"];
            Patient currentPatient = pdal.Patients.FirstOrDefault<Patient>(x => x.UserName == currentUser.UserName);
            AppointmentDal appDal = new AppointmentDal();
            Appointment update = appDal.Appointments.FirstOrDefault<Appointment>(x => x.Date == chosen.Date && x.DoctorName == chosen.DoctorName);
            update.PatientUserName = currentPatient.UserName;
            appDal.SaveChanges();
            return View("PatientPage");
        }
        public ActionResult YourAppointments()
        {
            if (!Authorize())
                return RedirectToAction("RedirectByUser", "Home");
            User curr = (User)Session["CurrentUser"];
            AppointmentViewModel AppVM = new AppointmentViewModel();
            AppointmentDal appDal = new AppointmentDal();
            AppVM.Appointments = (from app in appDal.Appointments
                                  where app.PatientUserName == curr.UserName && DateTime.Compare(DateTime.Now, app.Date) < 0
                                  select app).ToList<Appointment>();

            return View(AppVM);
        }
        public ActionResult GetYourAppointmentssByJson()
        {
            if (!Authorize())
                return RedirectToAction("RedirectByUser", "Home");
            User currentUser = (User)Session["CurrentUser"];
            AppointmentDal appDal = new AppointmentDal();
            PatientDal pdal = new PatientDal();
            Patient currentPatient = pdal.Patients.FirstOrDefault<Patient>(x => x.UserName == currentUser.UserName);
            List<Appointment> appointments = (from app in appDal.Appointments
                                              where app.PatientUserName == currentPatient.UserName && DateTime.Compare(DateTime.Now, app.Date) < 0
                                              select app).ToList<Appointment>();
            Thread.Sleep(1000);
            return Json(appointments, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CancelAppointment(string DoctorName, DateTime date)
        {
            if (!Authorize())
                return RedirectToAction("RedirectByUser", "Home");
            Appointment chosen = new Appointment { DoctorName = DoctorName, Date = date };
            AppointmentDal appDal = new AppointmentDal();
            Appointment update = appDal.Appointments.FirstOrDefault<Appointment>(x => x.Date == chosen.Date && x.DoctorName == chosen.DoctorName);
            update.PatientUserName = null;
            appDal.SaveChanges();
            DoctorDal dctDal = new DoctorDal();
            return View("PatientPage");

        }

        public ActionResult GetDoctorsByJson()
        {
            if (!Authorize())
                return RedirectToAction("RedirectByUser", "Home");
            User currentUser = (User)Session["CurrentUser"];
            DoctorDal docDal = new DoctorDal();
            List<string> doctors = (from doc in docDal.Users
                                    select doc.UserName).ToList<string>();
            for(int i=0;i<doctors.Count;i++)
            {
                doctors[i] = des.Decrypt(doctors[i], "Galit@19");
            }
            Thread.Sleep(1000);
            return Json(doctors, JsonRequestBehavior.AllowGet);
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
                VMm.Massages[i].SenderUserName = des.Decrypt(VMm.Massages[i].SenderUserName, "Galit@19");
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