using PropertyDetailsApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PropertyDetailsApi.Controllers
{
    public class ChatController : ApiController
    {
        public IEnumerable<conversation> getmessage()
        {
            IList<conversation> msg = null;
            using (capstoneProjectEntities1 db = new capstoneProjectEntities1())
            {
                msg = db.conversations.ToList();
            }
            return msg;
        }

        public IHttpActionResult postmessage(conversation msg)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Not an valid model");
            }
            using (var ctx = new capstoneProjectEntities1())
            {
                ctx.conversations.Add(new conversation()
                {
                    pid = msg.pid,
                    senderMail = msg.senderMail,
                    customermail = msg.customermail,
                    text = msg.text,
                    date = DateTime.Now,

                });
                ctx.SaveChanges();
            }
            return Ok();
        }
    }
}
