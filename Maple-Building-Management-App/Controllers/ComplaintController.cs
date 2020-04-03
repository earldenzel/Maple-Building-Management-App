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
        public ActionResult FileComplaint()
        {
            ComplaintModel model = new ComplaintModel();
            model.IncidentDate = DateTime.Today;
            model.ComplaintStatus = ComplaintStatus.Open.ToString();
            ViewBag.Message = "Create Complaint";

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

        public ActionResult ViewComplaints(string sortOrder, string searchString)
        {
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            int preview_max = 15;
            List<DataLibrary.Models.ComplaintModel> data = new List<DataLibrary.Models.ComplaintModel>();

            bool isAdmin = (String)Session["UserType"] == "A" ? true : false;

            if (isAdmin)
            {
                data = LoadComplaints();
            }
            else
            {
                if (Session["TenantID"] != null)
                {
                    data = LoadComplaintsByUserId((int)Session["TenantID"]);
                }
                else
                {
                    data = LoadComplaintsByPropertyManager((int)Session["PropertyID"]);
                }
            }

            bool showResolved = false;
            if (!String.IsNullOrEmpty(searchString))
            {
                switch (searchString.ToLower())
                {
                    case "open":
                        data = data.Where(s => s.ComplaintStatusId.Equals(1)).ToList();
                        break;
                    case "pending":
                        data = data.Where(s => s.ComplaintStatusId.Equals(2)).ToList();
                        break;
                    case "resolved":
                        data = data.Where(s => s.ComplaintStatusId.Equals(3)).ToList();
                        showResolved = true;
                        break;
                    case "reopened":
                        data = data.Where(s => s.ComplaintStatusId.Equals(4)).ToList();
                        break;
                    case "emergency":
                        data = data.Where(s => s.ComplaintTypeId.Equals(1)).ToList();
                        break;
                    case "pests":
                        data = data.Where(s => s.ComplaintTypeId.Equals(2)).ToList();
                        break;
                    case "maintenance":
                        data = data.Where(s => s.ComplaintTypeId.Equals(3)).ToList();
                        break;
                    case "noise":
                        data = data.Where(s => s.ComplaintTypeId.Equals(4)).ToList();
                        break;
                    default:
                        data = data.Where(s => s.Details.Contains(searchString)).ToList();
                        break;
                }
            }
            
            if (!showResolved)
            {
                data = data.Where(s => !s.ComplaintStatusId.Equals(3)).ToList();

            }

            switch (sortOrder)
            {
                case "Date":
                    data = data.OrderBy(s => s.IncidentDate).ToList();
                    break;
                case "date_desc":
                    data = data.OrderByDescending(s => s.IncidentDate).ToList();
                    break;
                default:
                    break;

            }
            

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

        //    public ActionResult ViewComplaints()
        //{
        //    int preview_max = 15;
        //    List<DataLibrary.Models.ComplaintModel> data = new List<DataLibrary.Models.ComplaintModel>();
        //    if (Session["TenantID"] != null)
        //    {
        //        data = LoadComplaintsByUserId((int)Session["TenantID"]);
        //    }
        //    else
        //    {
        //        data = LoadComplaintsByPropertyManager((int)Session["PropertyID"]);
        //    }

        //    List<ComplaintModel> complaints = new List<ComplaintModel>();

        //    foreach (var row in data)
        //    {
        //        string preview = row.Details.Substring(0, Math.Min(preview_max, row.Details.Length));
        //        if (row.Details.Length > preview_max)
        //        {
        //            preview += "...";
        //        }

        //        complaints.Add(new ComplaintModel
        //        {
        //            Id = row.Id,
        //            ComplaintStatus = Enum.GetName(typeof(ComplaintStatus), row.ComplaintStatusId),
        //            ComplaintType = Enum.GetName(typeof(ComplaintType), row.ComplaintTypeId),
        //            Description = preview,
        //            IncidentDate = row.IncidentDate,
        //        });
        //    }

        //    return View(complaints);
        //}

        //public ActionResult ViewAllComplaints()
        //{
        //    int preview_max = 15;
        //    var data = LoadComplaints();
        //    List<ComplaintModel> complaints = new List<ComplaintModel>();

        //    foreach (var row in data)
        //    {
        //        string preview = row.Details.Substring(0, Math.Min(preview_max, row.Details.Length));
        //        if (row.Details.Length > preview_max)
        //        {
        //            preview += "...";
        //        }

        //        complaints.Add(new ComplaintModel
        //        {
        //            Id = row.Id,
        //            ComplaintStatus = Enum.GetName(typeof(ComplaintStatus), row.ComplaintStatusId),
        //            ComplaintType = Enum.GetName(typeof(ComplaintType), row.ComplaintTypeId),
        //            Description = preview,
        //            IncidentDate = row.IncidentDate,
        //        });
        //    }

        //    return View(complaints);
        //}

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

        public ActionResult ResolveComplaint(int id)
        {
            int recordUpdated = UpdateComplaintStatus(
                id,
                (int)ComplaintStatus.Resolved
            );

            return RedirectToAction("ComplaintDetails", new { id = id });
        }

            public ActionResult EditComplaint(int id)
        {
            var data = LoadComplaint(id).FirstOrDefault();
            ComplaintModel complaint = new ComplaintModel();

            ////-
            //IEnumerable<SelectListItem> selectList = Enum.GetValues(typeof(ComplaintStatus));

            ////

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
    }
}