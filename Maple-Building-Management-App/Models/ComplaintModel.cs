using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace Maple_Building_Management_App.Models
{
    public class ComplaintModel
    {
        private DateTime incidentDate = DateTime.MinValue;

        [HiddenInput(DisplayValue = false)]
        public int ComplaintID { get; set; } //TODO: automatically create from database binding

        [Display(Name = "Type of Complaint")]
        public string ComplaintType { get; set; }
        public IEnumerable<SelectListItem> ComplaintTypes { get; set; }

        [Display(Name = "Description of the Complaint")]
        [Required(ErrorMessage = "Description of the complaint is required")]
        [StringLength(2000, MinimumLength = 1, ErrorMessage = "Please give a summary of the complaint (maximum 2000 letters)")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "When did this start?")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime IncidentDate
        {
            get
            {
                return (incidentDate == DateTime.MinValue) ? DateTime.Now : incidentDate;
            }
            set
            {
                incidentDate = value;
            }
        }

        [HiddenInput(DisplayValue = false)]
        [DataType(DataType.Date)]
        public DateTime ReportedDateTime { get; set; }
    }
}