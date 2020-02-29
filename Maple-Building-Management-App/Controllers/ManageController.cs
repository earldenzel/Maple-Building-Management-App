using Maple_Building_Management_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataLibrary.Models;
using AccountModel = DataLibrary.Models.AccountModel;
using System.Threading.Tasks;
using System.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.TwiML;
using Twilio.AspNet.Mvc;
using static DataLibrary.Logic.AccountProcessor;

namespace Maple_Building_Management_App.Controllers
{
    public class ManageController : TwilioController
    {
        // GET: Manage
        public ActionResult Index()
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.ErrorMessage = TempData["Error"];
            }

            ManageModel manageModel = new ManageModel();
            AccountModel dbModel = (AccountModel)Session["User"];
            manageModel.PhoneNumber = dbModel.PhoneNumber;
            manageModel.TwoFactor = dbModel.TwoFactor;
            return View(manageModel);
        }

        public ActionResult AddPhoneNumber()
        {
            ManageModel manageModel = new ManageModel();
            AccountModel dbModel = (AccountModel)Session["User"];
            manageModel.PhoneNumber = dbModel.PhoneNumber;
            return View(manageModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPhoneNumber(ManageModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = ((AccountModel)Session["User"]).Id; 
                ((AccountModel)Session["User"]).PhoneNumber = model.PhoneNumber;
                Session["Phone"] = model.PhoneNumber;
                return RedirectToAction("VerifyPhoneNumber");
            }
            return View();
        }

        public ActionResult VerifyPhoneNumber()
        {
            if (Session["Phone"] == null)
            {
                TempData["Error"] = "There is an error with your phone number. Please try adding again!";
                return RedirectToAction("Index");
            }
            if (Session["SentMessage"] == null || Session["ExpectedCode"] == null)
            {
                var accountSid = ConfigurationManager.AppSettings["SMSAccountIdentification"];
                var authToken = ConfigurationManager.AppSettings["SMSAccountPassword"];
                TwilioClient.Init(accountSid, authToken);

                var to = new PhoneNumber(Session["Phone"].ToString());
                var from = new PhoneNumber(ConfigurationManager.AppSettings["SMSAccountFrom"]);

                Random generator = new Random();
                string r = generator.Next(0, 999999).ToString("D6");
                Session["ExpectedCode"] = r;

                var message = MessageResource.Create(
                    to: to,
                    from: from,
                    body: "Your verification code for the Maple Building Management App is " + r);

                Session["SentMessage"] = message.Sid;
            }
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyPhoneNumber(ManageModel model)
        {
            if (ModelState.IsValid)
            {
                if (Session["SentMessage"] == null || Session["ExpectedCode"] == null || Session["Phone"] == null)
                {
                    TempData["Error"] = "Message has timed out. Please resend a new request to add phone number!";
                    return RedirectToAction("Login");
                }
                else if (string.Equals(model.VerificationCode, Session["ExpectedCode"].ToString()))
                {
                    model.Id = ((AccountModel)Session["User"]).Id;
                    model.PhoneNumber = Session["Phone"].ToString();
                    int recordsUpdated = UpdatePhoneNumber(model.Id, model.PhoneNumber);
                    return RedirectToAction("SuccessPhoneNumber");
                }
                else
                {
                    ViewBag.ErrorMessage = "Please input the correct verification code!";
                }
            }
            return View();
        }

        public ActionResult RemovePhoneNumber()
        {
            int id = ((AccountModel)Session["User"]).Id;
            ((AccountModel)Session["User"]).PhoneNumber = null;
            int recordsUpdated = DeletePhoneNumber(id);
            return View();
        }

        public ActionResult EnableTwoFactorAuthentication()
        {
            string phoneNumber = ((AccountModel)Session["User"]).PhoneNumber;
            if (phoneNumber == null)
            {
                TempData["Error"] = "Please add a phone number to the account to enable this feature!";
                return RedirectToAction("Index");
            }
            else
            {
                int id = ((AccountModel)Session["User"]).Id;
                ((AccountModel)Session["User"]).TwoFactor = true;
                int recordsUpdated = UpdateTwoFactor(id, true);
                return RedirectToAction("Index");
            }
        }
        public ActionResult DisableTwoFactorAuthentication()
        {
            int id = ((AccountModel)Session["User"]).Id;
            ((AccountModel)Session["User"]).TwoFactor = false;
            int recordsUpdated = UpdateTwoFactor(id, false);
            return RedirectToAction("Index");
        }
        
        public ActionResult SuccessPhoneNumber()
        {
            Session.Remove("Phone");
            Session.Remove("ExpectedCode");
            Session.Remove("SentMessage");
            return View();
        }
    }
}