using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Maple_Building_Management_App.Models
{
    public class ManageModel
    {
        public int Id { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }

        [StringLength(6, MinimumLength = 6, ErrorMessage = "This field must exactly 6 characters")]
        [RegularExpression(@"^[0-9]{1,6}$", ErrorMessage = "This field must contain numerical characters")]
        [Display(Name = "Verification Code")]
        public string VerificationCode { get; set; }

    }
}