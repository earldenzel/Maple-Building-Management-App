using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Maple_Building_Management_App.Models;
using DataLibrary;
using static DataLibrary.Logic.AccountProcessor;
using System.Threading.Tasks;
using System.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.TwiML;
using Twilio.AspNet.Mvc;
using System.Net.Mail;
using System.Net;

namespace Maple_Building_Management_App.Controllers
{
    public class HomeController : TwilioController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ViewAccounts()
        {
            ViewBag.Message = "Accounts List";

            var data = LoadAccounts();

            List<AccountModel> accounts = new List<AccountModel>();

            foreach (var row in data)
            {
                accounts.Add(new AccountModel
                {
                    FirstName = row.FirstName,
                    LastName = row.LastName,
                    EmailAddress = row.EmailAddress,
                    Tenant = row.Tenant,
                    PropertyCode = row.PropertyCode
                });
            }
            return View(accounts);
        }

        public ActionResult ViewTenantAccount()
        {
            ViewBag.Message = "Accounts List";
            var data = LoadAccounts();

            List<AccountModel> accounts = new List<AccountModel>();

            foreach (var row in data)
            {
                if (row.Tenant == true)
                {
                    accounts.Add(new AccountModel
                    {
                        FirstName = row.FirstName,
                        LastName = row.LastName,
                        EmailAddress = row.EmailAddress,
                        Tenant = row.Tenant,
                        PropertyCode = row.PropertyCode
                    });
                }
            }
            return View(accounts);
        }

        public ActionResult Register()
        {
            ViewBag.Message = "App Registration";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(AccountModel model)
        {
            DataLibrary.Models.AccountModel propertyManager = new DataLibrary.Models.AccountModel();
            if (ModelState.IsValid)
            {
                if (model.Tenant)
                {
                    if (model.PropertyCode == null)
                    {
                        ViewBag.ErrorMessage = "Please supply property code, when registering as a tenant!";
                        return View();
                    }

                    propertyManager = SearchPropertyManager(model.PropertyCode);
                    if (propertyManager == null)
                    {
                        ViewBag.ErrorMessage = "That property code is not a valid property code";
                        return View();
                    }
                }
                else
                {
                    Random random = new Random();
                    int value = random.Next(1000);
                    string text = value.ToString("000");
                    model.PropertyCode = (model.FirstName[0] + model.LastName + text).ToLower();
                }

                int recordsCreated = CreateAccount(
                    model.FirstName, 
                    model.LastName, 
                    model.EmailAddress, 
                    model.Password,
                    model.Tenant, 
                    model.PropertyCode);


                var senderEmail = new MailAddress("propertymbm@gmail.com");
                var receiverEmail = new MailAddress(model.EmailAddress);
                var password = "mapleB@1";
                var sub = "Welcome to the Maple Building Management App";
                string body = "";
                if (model.Tenant)
                {
                    body = "Good day, " + model.FirstName + "!" +
                        "\n\nWe are glad to have you with us!" +
                        "\n\nYou are now registered as a tenant under property managed by " + propertyManager.FirstName + " " + propertyManager.LastName + ". " +
                        "Please access the Maple Building Management web application through here: https://maple-building-management20200215053058.azurewebsites.net/ " +
                        "\n\nSincerely,\n\nMaple Building Management Admin";
                }
                else
                {
                    body = "Good day, " + model.FirstName + "!" +
                        "\n\nYou are now registered as a property manager with us! We are glad to have you onboard. " +
                        "We aim to be the premier solution for your building management needs." +
                        "\n\nYour property code is " + model.PropertyCode + 
                        "\n\nPlease keep a copy of this code with you. This code should be given to your tenants so they can access this application and help you " +
                        "manage the property you are renting to them! Please access the Maple Building Management web application through here: " +
                        "https://maple-building-management20200215053058.azurewebsites.net/ " +
                        "\n\nSincerely,\n\nMaple Building Management Admin";
                }
                
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password)
                };
                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = sub,
                    Body = body
                })
                {
                    smtp.Send(mess);
                }
                return View("SuccessfulRegister", model);
            }

            return View();
        }
        
        [AllowAnonymous]
        public ActionResult Login()
        {
            ViewBag.Message = "App Login";

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                List<DataLibrary.Models.AccountModel> accountModel = SearchAccount(
                    model.EmailAddress,
                    model.Password
                    );
                bool matchingFound = accountModel.Count > 0;

                if (matchingFound)
                {
                    DataLibrary.Models.AccountModel dbModel = accountModel.First();
                    if (dbModel.TwoFactor)
                    {
                        Session["UserID"] = dbModel.Id;
                        Session["Phone"] = dbModel.PhoneNumber;
                        return RedirectToAction("VerifyAccount");
                    }
                    else
                    {
                        Session["User"] = dbModel;
                        Session["UserID"] = dbModel.Id;
                        return RedirectToAction("ContentPage");
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Login Failed";
                    return View();
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Login Failed";
                return View();
            }
        }

        public ActionResult ContentPage()
        {
            //this is where all session vars are now set
            if (Session["User"] != null)
            {
                DataLibrary.Models.AccountModel dbModel = (DataLibrary.Models.AccountModel)Session["User"];
                Session["Admin"] = dbModel.Admin;

                if (dbModel.Tenant)
                {
                    Session["TenantID"] = dbModel.Id;
                    DataLibrary.Models.AccountModel propertyManagerModel = SearchPropertyManager(dbModel.PropertyCode);
                    Session["PropertyID"] = propertyManagerModel.Id;
                }
                else
                {
                    Session["PropertyID"] = dbModel.Id;
                }
            }
            return View();
        }

        public ActionResult VerifyAccount()
        {
            if (Session["SentMessageLogin"] == null || Session["ExpectedCodeLogin"] == null)
            {
                var accountSid = ConfigurationManager.AppSettings["SMSAccountIdentification"];
                var authToken = ConfigurationManager.AppSettings["SMSAccountPassword"];
                TwilioClient.Init(accountSid, authToken);


                var to = new PhoneNumber(Session["Phone"].ToString());
                var from = new PhoneNumber(ConfigurationManager.AppSettings["SMSAccountFrom"]);

                Random generator = new Random();
                string r = generator.Next(0, 999999).ToString("D6");
                Session["ExpectedCodeLogin"] = r;

                var message = MessageResource.Create(
                    to: to,
                    from: from,
                    body: "Your verification code for the Maple Building Management App is " + r);

                Session["SentMessageLogin"] = message.Sid;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyAccount(VerifyModel model)
        {
            if (ModelState.IsValid)
            {
                if (Session["SentMessageLogin"] == null || Session["ExpectedCodeLogin"] == null)
                {
                    TempData["Error"] = "Message has timed out. Please try to log in again";
                    return RedirectToAction("Index");
                }
                else if (string.Equals(model.VerificationCode, Session["ExpectedCodeLogin"].ToString()))
                {
                    Session["User"] = SearchAccount((int)Session["UserID"]);
                    Session.Remove("UserID");
                    Session.Remove("SentMessageLogin");
                    Session.Remove("ExpectedCodeLogin");
                    return RedirectToAction("ContentPage");
                }
                else
                {
                    ViewBag.ErrorMessage = "Please input the correct verification code!";
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Remove("User");
            Session.Remove("TenantID");
            Session.Remove("PropertyID");
            Session.Remove("Admin");
            return RedirectToAction("Index");
        }

        public ViewResult ComplainList()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ViewProfile()
        {
            ViewBag.Message = "Accounts List";
            AccountModel profile = new AccountModel();
                var data = LoadAccounts().FirstOrDefault();
                profile.FirstName = data.FirstName;
                profile.LastName = data.LastName;
                profile.EmailAddress = data.EmailAddress;
                profile.Tenant = data.Tenant;
                profile.PropertyCode = data.PropertyCode;

            
            return View(profile);
        }
        [HttpGet]
        public ActionResult EditProfile()
        {
            var data = LoadAccounts().FirstOrDefault();
            AccountModel profile = new AccountModel();

            profile.FirstName = data.FirstName;
            profile.LastName = data.LastName;
            profile.EmailAddress = data.EmailAddress;
            profile.Tenant = data.Tenant;
            profile.PropertyCode = data.PropertyCode;

            return View(profile);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditProfile(AccountModel profile)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        int recordsCreated = UpdateProfile(
        //            profile.FirstName,
        //            profile.LastName,
        //            profile.EmailAddress,
        //            profile.Tenant,
        //            profile.PropertyCode);

        //        return RedirectToAction("ViewProfile");
        //    }
        //    return View();
        //}
    }
}