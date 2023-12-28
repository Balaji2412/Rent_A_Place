using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapstoneProject.ViewModel
{
    public class Messages
    {
        public int pid { get; set; }
        public string senderMail { get; set; }
        public string customermail { get; set; }
        public string text { get; set; }
        public DateTime date{ get; set; }
    }
}