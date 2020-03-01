using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Maple_Building_Management_App.Models
{
    public class ChangePasswordModel
    {

        [Display(Name = "Old Password")]
        [Required(ErrorMessage = "Your old password is required.")]
        [DataType(DataType.Password)]
        [StringLength(25, MinimumLength = 8, ErrorMessage = "You password must be between 8 and 25 characters long")]
        public string Password { get; set; }

        [Display(Name = "New Password")]
        [Required(ErrorMessage = "Your new password is required.")]
        [DataType(DataType.Password)]
        [StringLength(25, MinimumLength = 8, ErrorMessage = "You password must be between 8 and 25 characters long")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Your password and confirm password do not match")]
        public string ConfirmNewPassword { get; set; }
    }
}