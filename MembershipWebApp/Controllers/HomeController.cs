using MembershipWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MembershipWebApp.Controllers
{
    public class HomeController : Controller
    {
        [Authorize(Roles = "Reader")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginUser user)
        {
            if(Membership.ValidateUser(user.Login, user.Password))
            {
                var roles = GetRoles(user.Login);
                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                  1,
                  user.Login,  //user id
                  DateTime.Now,
                  DateTime.Now.AddMinutes(20),  // expiry
                  false,  //do not remember
                  string.Join("|", roles),
                  "/");
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                                                   FormsAuthentication.Encrypt(authTicket));
                Response.Cookies.Add(cookie);
                return View("Index");
            }
            return View("BadPassword");
        }


        [HttpGet]
        public ActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateUser(LoginUser user)
        {
            var membershipUser = Membership.CreateUser(user.Login, user.Password);
            var memberGuid = membershipUser.ProviderUserKey;
            return View("About");
        }

        [Authorize(Roles = "Reader")]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Reader")]
        public ActionResult ChangePassword(ChangePasswordModel changePasswordModel)
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser != null)
            {
                membershipUser.ChangePassword(changePasswordModel.AncienPassword, changePasswordModel.NouveauPassword);
            }
            return View();
        }

        public ActionResult BadPassword()
        {
            ViewBag.Message = "Bad password";
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

        private List<string> GetRoles(string login)
        {
            var roles = new List<string>();
            using (var con = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                var cmd = new System.Data.SqlClient.SqlCommand(@"
SELECT R.[RoleName] 
FROM [aspnet_Roles] R 
INNER JOIN aspnet_UsersInRoles UR ON UR.RoleId = R.RoleId 
INNER JOIN aspnet_Users U ON U.UserId = UR.UserId
WHERE U.UserName='" + login + @"' ", con);
                var dt = new System.Data.DataTable();
                var sda = new System.Data.SqlClient.SqlDataAdapter(cmd);
                sda.Fill(dt);
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    roles.Add(dr["RoleName"].ToString());
                }
            }
            return roles;
        }

    }
}