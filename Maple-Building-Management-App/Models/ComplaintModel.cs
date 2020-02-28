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
        public int Id { get; set; }

        [Display(Name = "Type of Complaint")]
        [Required(ErrorMessage = "Complaint Type is required")]
        public string ComplaintType { get; set; }

        [Display(Name = "Complaint Status")]
        [Required(ErrorMessage = "Complaint Status is required")]
        public string ComplaintStatus { get; set; }

        [Display(Name = "Description of the Complaint")]
        [Required(ErrorMessage = "Description of the complaint is required")]
        [StringLength(2000, MinimumLength = 1, ErrorMessage = "Please give a summary of the complaint (maximum 2000 letters)")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "When did this start?")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime IncidentDate { get; set; } = DateTime.Now;
    }
}