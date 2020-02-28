using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Maple_Building_Management_App.Models;
using DataLibrary;
using static DataLibrary.Logic.AccountProcessor;
using System.Threading.Tasks;
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
                    model.Password,
                    model.Tenant, 
                    model.PropertyCode);
                return RedirectToAction("Index");
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
                //bool matchingFound = SearchAccount(
                //    model.EmailAddress,
                //    model.Password
                //    );
                bool matchingFound = SearchAccount(
                    model.EmailAddress,
                    model.Password
                    ).Count > 0;

                if (matchingFound)
                {
                    return RedirectToAction("ContentPage");
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
            return View();
        }

        public ActionResult ViewComplaints()
        {
            int preview_max = 15;
            var data = LoadComplaints();
            List<ComplaintModel> complaints = new List<ComplaintModel>();

            foreach (var row in data)
            {
                string preview = row.Details.Substring(0, Math.Min(preview_max, row.Details.Length));
                if (row.Details.Length > preview_max)
                {
                    preview += "...";
                }

                complaints.Add(new ComplaintModel
                {
                    Id = row.Id,
                    ComplaintType = Enum.GetName(typeof(ComplaintType), row.ComplaintTypeId),
                    ComplaintStatus = Enum.GetName(typeof(ComplaintStatus), row.ComplaintStatusId),
                    Description = preview,
                    IncidentDate = row.IncidentDate
                });
            }

            return View(complaints);
        }

        public ActionResult ComplaintDetails(int id)
        {
            var data = LoadComplaint(id).FirstOrDefault();
            ComplaintModel complaint = new ComplaintModel();
            complaint.Id = data.Id;
            complaint.ComplaintType = Enum.GetName(typeof(ComplaintType), data.ComplaintTypeId);
            complaint.ComplaintStatus = Enum.GetName(typeof(ComplaintStatus), data.ComplaintStatusId);
            complaint.Description = data.Details;
            complaint.IncidentDate = data.IncidentDate;

            return View(complaint);
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

        public ActionResult UpdateComplaint()
        {
            ComplaintModel model = new ComplaintModel();
            model.ComplaintStatus = ComplaintStatus.Open.ToString();
            ViewBag.Message = "Update Complaint";

            Session["TenantID"] = 1;
            Session["PropertyID"] = 2;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateComplaint(ComplaintModel model)
        {
            if (ModelState.IsValid)
            {
                int recordsCreated = CreateComplaint(
                    (int)Session["TenantID"],
                    (int)Session["PropertyID"],
                    model.IncidentDate,
                    model.Description,
                    (int)Enum.Parse(typeof(ComplaintStatus), model.ComplaintStatus),
                    (int)Enum.Parse(typeof(ComplaintType), model.ComplaintType)
                );
                return RedirectToAction("Index");
            }

            return View();
        }
    }
}