using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Maple_Building_Management_App.Models;
using DataLibrary;
using static DataLibrary.Logic.AccountProcessor;
using static DataLibrary.Logic.ComplaintProcessor;

namespace Maple_Building_Management_App.Controllers
{
    public class HomeController : Controller
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

        public ActionResult Register()
        {
            ViewBag.Message = "App Registration";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(AccountModel model)
        {
            if (ModelState.IsValid)
            {
                int recordsCreated = CreateAccount(
                    model.FirstName, 
                    model.LastName, 
                    model.EmailAddress, 
                    model.Tenant, 
                    model.PropertyCode);
                return RedirectToAction("Index");
            }

            return View();
        }

        public ActionResult FileComplaint()
        {
            ComplaintModel model = new ComplaintModel();
            model.IncidentDate = DateTime.Today;
            model.ComplaintStatus = ComplaintStatus.Open.ToString();
            ViewBag.Message = "Create Complaint";

            Session["TenantID"] = 1;
            Session["PropertyID"] = 2;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FileComplaint(ComplaintModel model)
        {
            if (ModelState.IsValid)
            {
                int recordsCreated = CreateComplaint(
                    (int)Session["TenantID"],
                    (int)Session["PropertyID"],
                    model.IncidentDate,
                    model.Description,
                    (int) Enum.Parse(typeof(ComplaintStatus), model.ComplaintStatus),
                    (int) Enum.Parse(typeof(ComplaintType), model.ComplaintType)
                );
                return RedirectToAction("Index");
            }

            return View();
        }
    }
}