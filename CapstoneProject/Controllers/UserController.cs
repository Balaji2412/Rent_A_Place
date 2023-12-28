using CapstoneProject.Models;
using CapstoneProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace CapstoneProject.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        //to view all the properties availabe

        //entity which holds user credentials.
        capstoneProjectEntities db1=new capstoneProjectEntities();

        capstoneProjectEntities2 db2=new capstoneProjectEntities2();    

        capstoneProjectEntities1 dbproperty=new capstoneProjectEntities1();

        public ActionResult Index() 
        {
            ViewBag.email = Session["Email"];
            return RedirectToAction("ViewProperty","user");
        }

        public ActionResult ViewProperty(string location, string features)
        {

            List<SelectListItem> availableLocations = new List<SelectListItem>();
            SelectListItem l1 = new SelectListItem() { Text = "Location", Value = "All", Selected = true };
            SelectListItem l2 = new SelectListItem() { Text = "Assam", Value = "Assam", Selected = false };
            SelectListItem l3 = new SelectListItem() { Text = "Hyderabad", Value = "Hyderabad", Selected = false };
            availableLocations.Add(l1);
            availableLocations.Add(l2);
            availableLocations.Add(l3);
            ViewBag.AvailableLocations = availableLocations;

            var a = Session["roles"];
            var email = Session["Email"];
            ViewBag.roles =a;

            IEnumerable<propertyDetail> pd = null;

            using (var client = new HttpClient())
            {
                if (location == null & features == null)
                {
                    client.BaseAddress = new Uri("http://localhost:62630/api/");

                    var responseTask = client.GetAsync("Propertydetails");
                    responseTask.Wait();


                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readtask = result.Content.ReadAsAsync<IList<propertyDetail>>();
                        readtask.Wait();

                        pd = readtask.Result;
                    }
                    else
                    {
                        pd = Enumerable.Empty<propertyDetail>();
                        ModelState.AddModelError(string.Empty, "server error contact admin");
                    }
                }
                if (location != null && features == null)
                {


                    string propertylocation = location.ToString();
                    client.BaseAddress = new Uri("http://localhost:62630/api/");

                    var responseTask = client.GetAsync("Propertydetails?location=" + propertylocation);
                    responseTask.Wait();


                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readtask = result.Content.ReadAsAsync<IList<propertyDetail>>();
                        readtask.Wait();

                        pd = readtask.Result;
                    }
                    else
                    {
                        pd = Enumerable.Empty<propertyDetail>();
                        ModelState.AddModelError(string.Empty, "server error contact admin");
                    }

                }
                if (location == null && features != null)
                {


                    string feature = features.ToString();
                    client.BaseAddress = new Uri("http://localhost:62630/api/");

                    var responseTask = client.GetAsync("Propertydetails?features=" + feature);
                    responseTask.Wait();


                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readtask = result.Content.ReadAsAsync<IList<propertyDetail>>();
                        readtask.Wait();

                        pd = readtask.Result;
                    }
                    else
                    {
                        pd = Enumerable.Empty<propertyDetail>();
                        ModelState.AddModelError(string.Empty, "server error contact admin");
                    }
                }


            }



            return View(pd);
        }

        //user update his profile.
        public ActionResult updataprofile() 
        {
            if (Session["roles"] != null)
            {
                int userid = Convert.ToInt32(Session["AdminID"]);
                if (Session["roles"] == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                credential uc = db1.credentials.Find(userid);
                if (uc == null)
                {
                    return HttpNotFound();
                }
                return View(uc);
            }
            return RedirectToAction("Login", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult updataprofile([Bind(Include = "userid,username,password,email,roles")] credential cd) 
        {
            if (ModelState.IsValid) 
            {
                db1.Entry(cd).State = EntityState.Modified;
                db1.SaveChanges();
                
                RedirectToAction("Index");
            }
            return View(cd);
        }
        //getting error
        //method for cart view

        //button has to be blocked if checkin and checkout has been confirmed.
        public ActionResult Cartview(int?pid) 
        {
            if (Session["roles"] != null)
            {
                var usermail = Session["Email"];
                if (pid == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                propertyDetail propertydata = dbproperty.propertyDetails.Find(pid);
                if (propertydata == null)
                {
                    return HttpNotFound();
                }
                return View(propertydata);
            }
            if(Session["roles"] == null)
            Content("<alert>' Please Login to Contineu....!'</alert>");
            return RedirectToAction("Login", "Home"); ;
        }

        //checkin checkout
       //checkin and checkout details for the property along with the property id and user id is stored in the database and an mail is send to the property owner.
        [HttpPost]
        public ActionResult cico(propertyDetail pd) 
        {
            if (Session["roles"] != null)
            {
                capstoneProjectEntities3 db = new capstoneProjectEntities3();
                propertyReservation pr = new propertyReservation();
                pr.userid = (int)Session["AdminID"];
                pr.propertyid = pd.propertyID;
                pr.adminid = pd.adminID;
                DateTime cki = Convert.ToDateTime(pd.datefrom);
                //pr.adminid = db =pd.adminID;
                pr.checkin = cki;
                pr.checkout = (DateTime)pd.dateto;
                pr.propertyname = pd.propertyName;

                db.propertyReservations.Add(pr);
                db.SaveChanges();
                TempData["msg"] = "<script>alert('An mail Has been Send to owner to confirm the reservation');</script>";
            }
            return RedirectToAction("Cartview", "User", new { pid=pd.propertyID});
            
        }



        //message to the propertyowner.

        public ActionResult sendmessage(int pid) 
        {
            var usermail = Session["Email"];
            int propertyid = pid;
            Session["pid"] = propertyid;
            ViewBag.email = usermail;
            ViewBag.propertyid = propertyid;

            //consuming api.
            IEnumerable<conversation> c = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:62630/api/");
                //httpget
                var responseTask = client.GetAsync("chat");
                responseTask.Wait();


                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readtask = result.Content.ReadAsAsync<IList<conversation>>();
                    readtask.Wait();

                    c = readtask.Result;
                }
                else
                {
                    c = Enumerable.Empty<conversation>();
                    ModelState.AddModelError(string.Empty, "server error contact admin");
                }
            }
            //return View(db2.conversations.ToList());
            return View(c);
        }

        public ActionResult storeMsg(FormCollection form) 
        {

            string name = form["message"];

            if (name != null) 
            {
                int pid = (int)Session["pid"];
                var usermail = Session["Email"];

                conversation cs = new conversation();
                cs.pid = pid;
                cs.senderMail = (string)usermail;
                cs.customermail= (string)usermail;
                cs.text = name;
                cs.date= DateTime.Now;
                db2.conversations.Add(cs);
                db2.SaveChanges();
                return RedirectToAction("sendmessage", "User", new { pid=pid});
            }

            return RedirectToAction("sendmessage", "User", new { pid = (int)Session["pid"] });
        }





    }
}