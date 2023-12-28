using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CapstoneProject.Models
{
    public class RegisterationModel
    {
        [Required]
        [Display(Name ="Name")]
        public string name { get; set; }

        [Required(ErrorMessage ="Please Enter Email")]
        [Display(Name ="Email")]
        [EmailAddress]
        public string email { get; set; }

        [Required(ErrorMessage ="please Enter password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }

        [Required(ErrorMessage = "please Enter password")]
        [DataType(DataType.Password)]
        [Compare("password",ErrorMessage ="Invalid password")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

    }
}