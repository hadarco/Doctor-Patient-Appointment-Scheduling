using AppointmentScheduling.Cryptography;
using AppointmentScheduling.DAL;
using AppointmentScheduling.Models;
using AppointmentScheduling.viewModel;
using System;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;

namespace AppointmentScheduling.Controllers
{
    public class HomeController : Controller
    {
        private DES des = new DES { };
        public ActionResult RedirectByUser()
        {
            if (Session["CurrentUser"] != null)
            {

                User currentUsr = (User)(Session["CurrentUser"]);
                DoctorDal docDal = new DoctorDal();
                if (docDal.Users.FirstOrDefault<Doctor>(x => x.UserName == currentUsr.UserName) != null)
                    return RedirectToAction("DoctorPage", "Doctor");
                else
                    return RedirectToAction("PatientPage", "Patient");
            }
            else
            {
                TempData["notAuthorized"] = "You have no permission!";
                return RedirectToAction("HomePage");
            }
        }
        public ActionResult HomePage()
        {
            des.Decrypt("?ÁjB", "Galit@19");
            if (Session["CurrentUser"] != null)
                return RedirectToAction("RedirectByUser");
            return View();
        }
        public ActionResult LoginPage()
        {
            if (Session["CurrentUser"] != null)
                return RedirectToAction("RedirectByUser");
            return View(new UserLogin());
        }

        public ActionResult AuthenticationPage(string AuthenticationCodeFromUser)
        {
            if (Session["randNum"] == null || Session["CurrentUserTemp"] == null)
                return RedirectToAction("RedirectByUser");
            string codeFromUser = AuthenticationCodeFromUser;
            string codeFromRandom = (string)Session["randNum"];
            if (codeFromUser == codeFromRandom)
            {
                Session["CurrentUser"] = Session["CurrentUserTemp"];
                Session["CurrentUserTemp"] = null;
                return RedirectToAction("RedirectByUser");
            }
            else
            {
                ViewBag.AuthenticationError = "Authentication failed, please try again.";
                return View();
            }
        }

        [HttpPost]
        public ActionResult Login(UserLogin usr)
        {
            if (Session["CurrentUser"] != null)
                return RedirectToAction("RedirectByUser");
            if (ModelState.IsValid)
            {
                UserDal usrDal = new UserDal();
                string encryptedUser = des.Encrypt(usr.UserName, "Galit@19");
                User objUser = usrDal.Users.FirstOrDefault<User>(x => x.UserName == encryptedUser);
                if (objUser == null || des.Decrypt(objUser.Password, "Galit@19") != usr.Password)
                {
                    ViewBag.errorUserLogin = "UserName or Password are incorrect";
                    return View("LoginPage", usr);
                }
                Patient pat = new PatientDal().Patients.FirstOrDefault<Patient>(x => x.UserName == encryptedUser);
                Session["CurrentUserTemp"] = objUser;
                string email = pat.PatientEmail;

                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                string stringChars = "";
                Random rnd = new Random();

                for (int i = 0; i < 6; i++)
                {
                    stringChars += chars[rnd.Next(chars.Length)];
                }

                //int randNum = rnd.Next(10000, 100000);
                Session["randNum"] = stringChars;
                string body = "Authentication number is: " + stringChars.ToString() + " .";
                string topic = "Authentication Code for Medical-Calendar";
                SendMail(body, topic, email);
                return View("AuthenticationPage");
                //return RedirectToAction("RedirectByUser");
            }
            else
            {
                usr.Password = "";
                return View("LoginPage", usr);
            }
        }
        public ActionResult SignupPage()
        {
            if (Session["CurrentUser"] != null)
                return RedirectToAction("RedirectByUser");
            return View(new VMUserRegister());
        }
        public ActionResult Register()
        {
            if (Session["CurrentUser"] != null)
                return RedirectToAction("RedirectByUser");
            VMUserRegister newUsr = new VMUserRegister();
            newUsr.NewUser = new User();
            return View("SignupPage", newUsr);
        }
        [HttpPost]
        public ActionResult RegisterCon(VMUserRegister usr)
        {
            string encryptedAnswer = AES.Encrypt(usr.NewUser.SecurityAnswer);
            if (Session["CurrentUser"] != null)
                return RedirectToAction("RedirectByUser");

            usr.NewUser.UserName = des.Encrypt(usr.UserName, "Galit@19");
            usr.NewUser.Password = des.Encrypt(usr.Password, "Galit@19");

            ModelState.Clear();
            TryValidateModel(usr);

            if (ModelState.IsValid)
            {
                UserDal usrDal = new UserDal();
                PatientDal ptntDal = new PatientDal();
                string encryptedUser = des.Encrypt(usr.UserName, "Galit@19");
                User objUser = usrDal.Users.FirstOrDefault<User>(x => x.UserName == encryptedUser);
                if (objUser != null)
                {
                    ViewBag.errorUserRegister = "The user name is already exist";
                    return View("SignupPage", usr);
                }

                usr.NewUser.SecurityAnswer = encryptedAnswer;
                usr.NewUser.SecurityQuestion = Request.Form["sq"];
                usr.PatientDetails.UserName = des.Encrypt(usr.UserName, "Galit@19");
                usrDal.Users.Add(usr.NewUser);
                ptntDal.Patients.Add(usr.PatientDetails);
                ptntDal.SaveChanges();
                usrDal.SaveChanges();
                ViewBag.registerSuccessMsg = "Your registration has been succeeded!";
                return View("HomePage", usr.NewUser);
            }
            else
            {
                usr.Password = "";
                return View("SignupPage", usr);
            }
        }
        public ActionResult Logout()
        {
            Session["CurrentUser"] = null;
            return RedirectToAction("HomePage");
        }
        public ActionResult ResetPass()
        {
            if (Session["CurrentUser"] != null)
                return RedirectToAction("RedirectByUser");
            return View();
        }
        public ActionResult CheckUser(string userName)
        {
            if (Session["CurrentUser"] != null)
                return RedirectToAction("RedirectByUser");
            UserDal usrDal = new UserDal();
            string encryptedUsername = des.Encrypt(userName, "Galit@19");
            User user = usrDal.Users.FirstOrDefault<User>(x => x.UserName == encryptedUsername);
            if (user == null)
            {
                ViewBag.errorUserLogin = "User Name doesn't exist";
                return RedirectToAction("ResetPass");
            }
            return View(user);
        }
        [HttpPost]
        public ActionResult SendEmail(User user)
        {
            if (Session["CurrentUser"] != null)
                return RedirectToAction("RedirectByUser");
            string ans = user.SecurityAnswer;
            UserDal userdal = new UserDal();
            user = userdal.Users.FirstOrDefault<User>(x => x.UserName == user.UserName);
            if (ans != AES.Decrypt(user.SecurityAnswer))
            {
                ViewBag.ans = "The answer is incorrect!";
                return RedirectToAction("LoginPage");
            }
            SendNewPass(user);
            userdal.SaveChanges();
            return RedirectToAction("LoginPage");
        }

        private void SendMail(string msg, string topic, string email)
        {
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("medicalcalendar123", "mc12345!");
            try //Mail built-in  validation function.
            {
                MailAddress m = new MailAddress(email);
            }

            catch (FormatException)
            {
                Console.WriteLine("Are you sure that you entered a valid mail address? Try again please.");
                return;
            }
            MailMessage mm = new MailMessage("medicalcalendar123@donotreply.com", email, topic, msg);
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.Send(mm);
        }

        private void SendNewPass(User user)
        {

            Patient pat = new PatientDal().Patients.FirstOrDefault<Patient>(x => x.UserName == user.UserName);
            string email = pat.PatientEmail;
            Random rnd = new Random();
            int randNum = rnd.Next(10000, 100000);
            string body = "Temporary password is: " + randNum.ToString() + " ." + "\nPlease change your password as soon as possible.";
            string topic = "Password Reset for Medical-Calendar";
            SendMail(body, topic, email);
            user.Password = des.Encrypt(randNum.ToString(), "Galit@19");
        }

    }
}