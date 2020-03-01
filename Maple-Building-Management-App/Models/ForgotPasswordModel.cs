using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Maple_Building_Management_App.Models
{
    public class ForgotPasswordModel
    {
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Your email address is required.")]
        public string EmailAddress { get; set; }
    }
}