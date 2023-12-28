using CapstoneProject.Models;
using CapstoneProject.ViewModel;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace CapstoneProject.Controllers
{
    public class AdminController : Controller
    {
        capstoneProjectEntities userdetails=new capstoneProjectEntities();//details of credentials
        capstoneProjectEntities1 db = new capstoneProjectEntities1();//holds table of property details
        capstoneProjectEntities2 db2= new capstoneProjectEntities2();//table for chat
        capstoneProjectEntities3 reserve = new capstoneProjectEntities3();//details of property reservation

        // GET: Admin
        public ActionResult Index()
        {
           // ViewBag.role = Session["roles"];
            return RedirectToAction("viewProperty","Admin");
        }

        public ActionResult CreateProperty() 
        {
            var aid = Session["AdminId"];
            if (Session["AdminID"] != null)
            {
                return View();
            }
            else 
            {
                return RedirectToAction("Login", "Home");
            }
        }
        [HttpPost]
        public ActionResult CreateProperty(propertyView pd, HttpPostedFileBase[] files)
        {
            if (ModelState.IsValid)
            {
                propertyDetail p = new propertyDetail();

                p.adminID = Convert.ToInt32(Session["AdminID"]);
                p.adminName = Session["UserName"].ToString();
                p.propertyName = pd.propertyName;
                p.Location = pd.Location;
                p.price = pd.price;
                p.category = pd.category;
                try
                {
                    if (files != null)
                    {
                        string e = Path.GetExtension(files.ToString());

                        //if (e.ToLower().Equals(".HttpPostedFileBase[]")) { }
                        List<string> fname = new List<string>();
                        foreach (HttpPostedFileBase file in files)
                        {
                            // bool f= Path.HasExtension(file.IfNotNull)
                            bool b1 = string.IsNullOrEmpty(file.FileName);
                            if (b1 == false)
                            {
                                string extensions = Path.GetExtension(file.FileName);
                                if (extensions.ToLower().Equals(".jpg") || extensions.ToLower().Equals(".jpeg") || extensions.ToLower().Equals(".png"))
                                {
                                    fname.Add(file.FileName);
                                    var SavePath = Path.Combine(Server.MapPath("~/Images/") + Path.GetFileName(file.FileName));
                                    file.SaveAs(SavePath);
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Image formate should be only .jpg,.jpeg,.png");
                                }
                            }

                        }
                        if (fname.Count > 0)
                        {
                            string filename = string.Join(",", fname);
                            p.imageName = filename;
                        }
                    }

                }
                catch(Exception) 
                { }

                db.propertyDetails.Add(p);
                db.SaveChanges();

                return RedirectToAction("viewProperty", "Admin");
            }
            else
            {
                ModelState.AddModelError("", "Something went wrong please try again later!");
            }
            return RedirectToAction("viewProperty", "Admin");
        }


        //to view all the properties availabe in the database.
        //only specific admins property is visible in the page.


        //for viewproperty view is created with the model propertyDetais from entity.

        //in the view part of the admin the property rating is not shown because the rating part is not yet done from the user side.
        public ActionResult viewProperty() 
        {
            if (Session["AdminID"] != null)
            {
                capstoneProjectEntities1 db = new capstoneProjectEntities1();

                int adminid = (int)Session["AdminID"];

                var details = db.propertyDetails
                    .Where(c => c.adminID == adminid)
                    .ToList();

                var reservationcount = reserve.propertyReservations
                    .Where(r => r.adminid == adminid && r.confirmation == null)
                    .Count();
                ViewBag.count = reservationcount;

                return View(details);

               
            }
            else 
            {
                return RedirectToAction("Login", "Home");
            }
        }


        //method to add images pif property owner wants to add extra images to the property.
        public ActionResult viewImages(int pid, HttpPostedFileBase[] files) 
        {
            if (Session["AdminID"] != null)
            {
                //adding new images if owner needes to add additional images to the property.
                propertyDetail pd = new propertyDetail();

                //display  existing images.
                int propertyid = pid;
                ViewBag.propertyid = propertyid;
                var Data = db.propertyDetails
                    .Where(c => c.propertyID == propertyid)
                    .FirstOrDefault();

                //Retriving the image details from the view and adding them to the model.
                List<string> fname = new List<string>();
                if (ModelState.IsValid)
                {
                    if (files != null)
                    {

                        foreach (HttpPostedFileBase file in files)
                        {
                            // bool f= Path.HasExtension(file.IfNotNull)
                            bool b1 = string.IsNullOrEmpty(file.FileName);
                            if (b1 == false)
                            {
                                string extensions = Path.GetExtension(file.FileName);
                                if (extensions.ToLower().Equals(".jpg") || extensions.ToLower().Equals(".jpeg") || extensions.ToLower().Equals(".png"))
                                {
                                    fname.Add(file.FileName);
                                    var SavePath = Path.Combine(Server.MapPath("~/Images/") + Path.GetFileName(file.FileName));
                                    file.SaveAs(SavePath);
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Image formate should be only .jpg,.jpeg,.png");
                                }
                            }
                        }
                        if (fname.Count > 0)
                        {
                            string filename = string.Join(",", fname);
                            pd.imageName = filename;
                        }
                    }
                }

                //adding the retrived model to the database
                //adding images for the particular property.

                if (Data.imageName == null)
                {
                    Data.imageName = pd.imageName;
                    db.SaveChanges();
                }
                else
                {
                    string dbimage = Data.imageName;
                    char[] seperate = { ',' };
                    string[] imgarray = dbimage.Split(seperate);
                    List<string> l1 = imgarray.ToList();

                    foreach (var iname in fname)
                    {
                        l1.Add(iname);
                    }

                    string joinName = string.Join(",", l1);
                    Data.imageName = joinName;
                    db.SaveChanges();

                }

                //for displaying images.
                // string imageData;
                if (Data.imageName != null)
                {
                    string imageData = Data.imageName;
                    char[] seperate = { ',' };

                    string[] images = imageData.Split(seperate);

                    ViewBag.imageName = images;
                    return View();
                }

            }
            return View();
        }

       
        //To delete the images which are available for the property
        public ActionResult DeletePropertyImage(string img,int pid) 
        {
            if (Session["AdminID"] != null)
            {
                int propertyid = pid;

                var Data = db.propertyDetails
                    .Where(c => c.propertyID == propertyid)
                    .FirstOrDefault();
                if (Data != null)
                {
                    //fetching image daata from the database as a string 
                    string imageData = Data.imageName;
                    //spliting that comma seperated values (imagedata)
                    char[] seperate = { ',' };
                    //storing the data in an string array.
                    string[] images = imageData.Split(seperate);
                    //converting the array to string.
                    List<string> listimages = images.ToList();

                    if (listimages.Contains(img))
                    {
                        listimages.Remove(img);
                    }
                    //again adding the comma seperated values to the string.
                    if (listimages.Count > 0)
                    {
                        string fname = string.Join(",", listimages);
                        Data.imageName = fname;
                        db.SaveChanges();
                    }
                }
            }
                return RedirectToAction("viewImages", "Admin", new { @pid=pid});
        }

        //Deleting the propery from the database.
        
        public ActionResult DeleteProperty(int pid) 
        {
            if (Session["AdminID"] != null)
            {
                int propertyid = pid;


                var propertyDelete = db.propertyDetails
                    .Where(d => d.propertyID == propertyid)
                    .FirstOrDefault();
                db.propertyDetails.Remove(propertyDelete);
                db.SaveChanges();
            }
            return RedirectToAction("viewProperty", "Admin");
        }

        //editProperty method is used to edit the property details if the admin wishes to change anything.
        public ActionResult editProperty(int? pid)
        {
            if (pid == null) 
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            propertyDetail pd = db.propertyDetails.Find(pid);
            
            if(pd == null) 
            {
                return HttpNotFound();
            }
            
            return View(pd);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editProperty([Bind(Include = "propertyID,adminID,adminName,propertyName,Location,price,category,imageName,Rating")] propertyDetail pd) 
        {

            if (ModelState.IsValid) 
            {
                db.Entry(pd).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("viewProperty");
            }
            return View(pd);
        }

        public ActionResult getProperties(int? pid) 
        {
            if (pid == null) 
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            propertyDetail pd=db.propertyDetails.Find(pid);

            if (pd.imageName != null) 
            {
                string imageData = pd.imageName;
                char[] seperate = { ',' };

                string[] images = imageData.Split(seperate);

                ViewBag.imageName = images;
            }
            
            if (pd == null) 
            {
                return HttpNotFound();
            }
            return View(pd);
        }


        //view all the messages came to the particular property.
        public ActionResult allChats(int pid) 
        {
            //propertyowner emailid.
            var propertyEmail = Session["Email"];
            //propertyid
            int propertId = pid;

            ViewBag.propertId = propertId;
            ViewBag.owneremail = propertyEmail;

            return View(db2.conversations.ToList());
        }

        //view or replay to the particular user sent to particular property.
        public ActionResult chat(string UserMail, string OwnerMail, int PId)
        {
            ViewBag.UserMail = UserMail;

            Session["useremail"] = UserMail;

            ViewBag.OwnerMail = OwnerMail;
            ViewBag.PId = PId;
            Session["pid"] = PId;
            var um = Session["useremail"];
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

        //send message to the user. by writing in the database.
        public ActionResult AdminWriteMessage(FormCollection form)
        {
            if (form != null)
            {
                var um = Session["useremail"];
                var sm = Session["Email"];
                var pid = Session["pid"];

                string name = form["message"];

                ViewBag.OwnerMail = um;
                ViewBag.SenderMail = sm;
                ViewBag.PId = pid;

                conversation cv = new conversation();
                cv.customermail = (string)um;
                cv.senderMail = (string)sm;
                cv.date = DateTime.Now;
                cv.text = name;
                cv.pid = (int)Session["pid"];
                db2.conversations.Add(cv);
                db2.SaveChanges();
                

                return RedirectToAction("chat", "Admin", new { UserMail=um, OwnerMail =sm, PId =pid});
            }
            return RedirectToAction("chat", "Admin", new { OwnerMail = Session["Email"], UserMail = Session["useremail"], PId = Session["pid"] });
        }

        public ActionResult confirmReservation()
        {
            int adminid = (int)Session["AdminID"];
            var reservation = reserve.propertyReservations
                .Where(r => r.adminid == adminid)
                .ToList();

            return View(reservation); 
        }
        //accept reservation
        public ActionResult Accept(int?rid) 
        {
            if (rid == null) 
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var reservation = reserve.propertyReservations.Where(u => u.reservationId == rid).FirstOrDefault();
            if (reservation.confirmation == null) 
            {
                string confirmation = "Accepted";
                reservation.confirmation = confirmation;
                reserve.Entry(reservation).State= EntityState.Modified;
                reserve.SaveChanges();

            }
            return RedirectToAction("confirmReservation");
        }
        //Declain

        public ActionResult Declain(int? rid)
        {
            if (rid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var reservation = reserve.propertyReservations.Where(u => u.reservationId == rid).FirstOrDefault();
            if (reservation.confirmation == null)
            {
                string confirmation = "Declined";
                reservation.confirmation = confirmation;
                reserve.Entry(reservation).State = EntityState.Modified;
                reserve.SaveChanges();

            }
            return RedirectToAction("confirmReservation");
        }

    }
}