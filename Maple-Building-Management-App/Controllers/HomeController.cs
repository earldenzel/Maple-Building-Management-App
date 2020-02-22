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

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Login(AccountModel model, string returnUrl)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // Require the user to have a confirmed email before they can log on.
        //    var user = await UserManager.FindByNameAsync(model.EmailAddress);
        //    if (user != null)
        //    {
        //        if (!await UserManager.IsEmailConfirmedAsync(user.Id))
        //        {
        //            ViewBag.errorMessage = "You must have a confirmed email to log on.";
        //            return View("Error");
        //        }
        //    }

        //    // This doesn't count login failures towards account lockout
        //    // To enable password failures to trigger account lockout, change to shouldLockout: true
        //    var result = await SignInManager.PasswordSignInAsync(model.EmailAddress, model.Password, shouldLockout: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(returnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
        //        case SignInStatus.Failure:
        //        default:
        //            ModelState.AddModelError("", "Invalid login attempt.");
        //            return View(model);
        //    }
        //}

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
                    return RedirectToAction("Index");
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