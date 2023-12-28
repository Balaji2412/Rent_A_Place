using CapstoneProject.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CapstoneProject.Controllers
{
    public class HomeController : Controller
    {

      
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult UserRegister() 
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
        public ActionResult UserRegister(RegisterationModel register)
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
                        cd.roles = "User";
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



        //login for both user and admin and redirecting them accordint to their role.
        public ActionResult Login()
        { 
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel loginModel)
        {
            string Email = loginModel.eMail;
            string Password = loginModel.password;
            if (ModelState.IsValid)
            {
                SqlConnection connection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=capstoneProject;Integrated Security=True");
                SqlDataAdapter myadapter = new SqlDataAdapter();
                string sql = $"select userid,username,email,roles from credential where email='{Email}'and password='{Password}'";
                connection.Open();
                SqlCommand cmd=new SqlCommand(sql, connection);
                SqlDataReader reader= cmd.ExecuteReader();
                if (reader.Read()) 
                {
                    string role1 = "SuperAdmin";
                    string role2 = "Admin";
                    string role3 = "User";
                    if (reader["roles"].ToString().ToLower() ==role1.ToLower()) 
                    {
                        Session["roles"] = reader["roles"];
                        //userid
                        Session["AdminID"] = reader["userid"];
                        Session["UserName"] = reader["username"];
                        Session["Email"] = reader["email"];
                        return RedirectToAction("Index", "SuperAdmin");
                    }
                    if (reader["roles"].ToString().ToLower() == role2.ToLower()) 
                    {
                        Session["AdminID"] = reader["userid"];
                        Session["UserName"] = reader["username"];
                        Session["Email"] = reader["email"];
                        Session["roles"] = reader["roles"];
                        return RedirectToAction("Index", "Admin");
                    }
                    if (reader["roles"].ToString().ToLower() ==role3.ToLower())
                    {
                        Session["AdminID"] = reader["userid"];
                        Session["UserName"] = reader["username"];
                        Session["Email"] = reader["email"];
                        var email = Session["Email"];
                        Session["roles"] = reader["roles"];
                        return RedirectToAction("Index", "User");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "entered user id and password in wrong");
                }
                connection.Close();
              
            }

                return View();
        }

        public ActionResult logout()
        {
            Session.Clear();
            return RedirectToAction("ViewProperty", "User");
        }


    }
}