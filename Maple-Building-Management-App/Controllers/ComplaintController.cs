using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Maple_Building_Management_App.Models;
using static DataLibrary.Logic.ComplaintProcessor;
using AccountModel = DataLibrary.Models.AccountModel;

namespace Maple_Building_Management_App.Controllers
{
    public class ComplaintController : Controller
    {
        //// GET: Complaint
        //public ActionResult Index()
        //{
        //    return View();
        //}


        public ActionResult FileComplaint()
        {
            ComplaintModel model = new ComplaintModel();
            model.IncidentDate = DateTime.Today;
            model.ComplaintStatus = ComplaintStatus.Open.ToString();
            ViewBag.Message = "Create Complaint";

            SetSessionVars();

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
                    (int)Enum.Parse(typeof(ComplaintStatus), model.ComplaintStatus),
                    (int)Enum.Parse(typeof(ComplaintType), model.ComplaintType)
                );
                return View("SuccessfulComplaint", model);
            }

            return View();
        }

        public ActionResult ViewComplaints()
        {
            int preview_max = 15;
            SetSessionVars();
            var data = LoadComplaintsByUserId((int)Session["TenantID"]);
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
                    ComplaintStatus = Enum.GetName(typeof(ComplaintStatus), row.ComplaintStatusId),
                    ComplaintType = Enum.GetName(typeof(ComplaintType), row.ComplaintTypeId),
                    Description = preview,
                    IncidentDate = row.IncidentDate,
                });
            }

            return View(complaints);
        }

        public ActionResult ViewAllComplaints()
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
                    ComplaintStatus = Enum.GetName(typeof(ComplaintStatus), row.ComplaintStatusId),
                    ComplaintType = Enum.GetName(typeof(ComplaintType), row.ComplaintTypeId),
                    Description = preview,
                    IncidentDate = row.IncidentDate,
                });
            }

            return View(complaints);
        }

        public ActionResult ComplaintDetails(int id)
        {
            var data = LoadComplaint(id).FirstOrDefault();
            ComplaintModel complaint = new ComplaintModel();

            complaint.Id = data.Id;
            complaint.ComplaintStatus = Enum.GetName(typeof(ComplaintStatus), data.ComplaintStatusId);
            complaint.ComplaintType = Enum.GetName(typeof(ComplaintType), data.ComplaintTypeId);
            complaint.Description = data.Details;
            complaint.IncidentDate = data.IncidentDate;
            complaint.Feedback = data.Feedback;

            return View(complaint);
        }

        public ActionResult EditComplaint(int id)
        {
            var data = LoadComplaint(id).FirstOrDefault();
            ComplaintModel complaint = new ComplaintModel();

            complaint.Id = data.Id;
            complaint.ComplaintStatus = Enum.GetName(typeof(ComplaintStatus), data.ComplaintStatusId);
            complaint.ComplaintType = Enum.GetName(typeof(ComplaintType), data.ComplaintTypeId);
            complaint.Description = data.Details;
            complaint.IncidentDate = data.IncidentDate;
            complaint.Feedback = data.Feedback;

            return View(complaint);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditComplaint(ComplaintModel model)
        {
            if (ModelState.IsValid)
            {
                int recordUpdated = UpdateComplaint(
                    model.Id,
                    model.IncidentDate,
                    model.Description,
                    (int)Enum.Parse(typeof(ComplaintStatus), model.ComplaintStatus),
                    (int)Enum.Parse(typeof(ComplaintType), model.ComplaintType),
                    model.Feedback
                );
                return RedirectToAction("ViewComplaints");
            }

            return View();
        }

        public ActionResult DeleteComplaint(int id)
        {
            int recordDeleted = DeleteComplaintData(id);

            return RedirectToAction("ViewComplaints");
        }

        public void SetSessionVars()
        {
            AccountModel dbModel = (AccountModel)Session["User"];
            Session["TenantID"] = dbModel.Id;
            Session["PropertyID"] = 2;
        }
    }
}