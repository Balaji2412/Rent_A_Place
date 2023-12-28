using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CapstoneProject.ViewModel
{
    public class propertyView
    {
        [Key]
        public int propertyID { get; set; }

        
        public int adminID { get; set; }
        public string adminName { get; set; }
        [Required]
        [Display(Name = "Property Name")]
        public string propertyName { get; set; }
        [Required]
        [Display(Name = "Property Location")]
        public string Location { get; set; }

        [Required]
        public int price { get; set; }
        [Required]
        public string category { get; set; }

        
        [Display(Name = "Browse File")]
        public HttpPostedFileBase[] files { get; set; }
    }
}