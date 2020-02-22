using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Maple_Building_Management_App.Models
{
    public class LoginModel
    {
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Your email address is required.")]
        public string EmailAddress { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Your password is required.")]
        [DataType(DataType.Password)]
        [StringLength(25, MinimumLength = 8, ErrorMessage = "You password must be between 8 and 25 characters long")]
        public string Password { get; set; }
    }
}