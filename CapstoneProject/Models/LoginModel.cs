using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CapstoneProject.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        [Display(Name ="Email ")]
        public string eMail { get; set; }

        [Required(ErrorMessage = "please Enter password")]
        [DataType(DataType.Password)]
        [Display(Name ="Password")]
        public string password { get; set; }
    }
}