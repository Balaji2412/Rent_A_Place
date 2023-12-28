using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapstoneProject.Models
{
    public class usermodel
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string roles { get; set; }
    }
}