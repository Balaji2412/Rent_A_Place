using CapstoneProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CapstoneProject.Controllers
{
    public class SuperAdminController : Controller
    {
        // GET: SuperAdmin
        public ActionResult Index()
        {
            ViewBag.role = Session["roles"];
            return View();
        }
        
        //only super admin can register for admin and provide credentials for the admin.
        //registeration is done by the superadmin(similar to the owner and employees)
        public ActionResult AdminRegister() 
        {
            return View();
        }

        //check if email exist or not in the database.
        private bool isEmailExist(string email)
        {
            bool isEmailExist = false;
            using (capstoneProjectEntities db = new capstoneProjectEntities())
            {
                int count = db.credentials
                    .Where(a => a.email == email).Count();
                if (count > 0)
                {
                    isEmailExist = true;
                }
            }
            return isEmailExist;
        }
        
        [HttpPost]
        public ActionResult AdminRegister(RegisterationModel register)
        {
            if (ModelState.IsValid)
            {
                if (!isEmailExist(register.email))
                {
                    using (capstoneProjectEntities db = new capstoneProjectEntities())
                    {
                        credential cd = new credential();
                        cd.username = register.name;
                        cd.password = register.password;
                        cd.email = register.email;
                        cd.roles = "Admin";
                        db.credentials.Add(cd);

                        if (db.SaveChanges() > 0)
                        {
                            FormsAuthentication.SetAuthCookie(register.email, false);
                            return RedirectToAction("Login", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Something went wrong please try again later!");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email Already Exist \n Enter new Email");
                }
            }
            else
            {
                ModelState.AddModelError("", "Model State is Invalid");
            }
            return View();
        }
    }
}