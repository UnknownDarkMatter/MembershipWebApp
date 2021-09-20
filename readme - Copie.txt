
#### utilisation app
ouvrir /Home/CreateUSer et créer un user dans la BDD ajouter le role Reader
essayer d'ouvrir /Home/Index

#### installation app

créer web application .Net Framework MVC
créer BDD

modif web.config
  <connectionStrings>
	<add name="DefaultConnection" 
		 connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=MembershipWebApp;Integrated Security=SSPI;"
		 providerName="System.Data.SqlClient" />
  </connectionStrings>
  <system.web>
    <compilation debug="true" targetFramework="4.7.2" />
    <httpRuntime targetFramework="4.7.2" />
	  <membership defaultProvider="MembershipProvider" userIsOnlineTimeWindow="15">
		  <providers>
			  <clear />
			  <add name="MembershipProvider"
					type="System.Web.Security.SqlMembershipProvider"
					connectionStringName="DefaultConnection"
					applicationName="MembershipWebApp"
					enablePasswordRetrieval="false"
					enablePasswordReset="true"
					requiresQuestionAndAnswer="false"
					requiresUniqueEmail="true"
					passwordFormat="Hashed"
					minRequiredNonalphanumericCharacters="0"  />
		  </providers>
	  </membership>
  </system.web>
  
cmd : 
	C:\Windows\Microsoft.NET\Framework\v4.0.30319\aspnet_regsql.exe

web.config
  <connectionStrings>
	<add name="DefaultConnection" 
		 connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=MembershipWebApp;Integrated Security=SSPI;"
		 providerName="System.Data.SqlClient" />
  </connectionStrings>
  <system.web>
    <compilation debug="true" targetFramework="4.7.2" />
    <httpRuntime targetFramework="4.7.2" />
	  <membership defaultProvider="MembershipProvider" userIsOnlineTimeWindow="15">
		  <providers>
			  <clear />
			  <add name="MembershipProvider"
					type="System.Web.Security.SqlMembershipProvider"
					connectionStringName="DefaultConnection"
					applicationName="MembershipWebApp"
					enablePasswordRetrieval="false"
					enablePasswordReset="true"
					requiresQuestionAndAnswer="false"
					requiresUniqueEmail="false"
					passwordFormat="Hashed"
					minRequiredNonalphanumericCharacters="0"
				    minRequiredPasswordLength="6"
				   />
		  </providers>
	  </membership>
	  <authentication mode="Forms">
		  <forms loginUrl="/Home/Login" defaultUrl="/Home/Index" timeout="15" cookieless="UseCookies" />
	  </authentication>
	  <roleManager enabled="true">
		  <providers>
			  <clear/>
			  <add name="AspNetSqlRoleProvider" connectionStringName="DefaultConnection" applicationName="MembershipWebApp" type="System.Web.Security.SqlRoleProvider"/>
		  </providers>
	  </roleManager>
  </system.web>
  
Global.asax.cs  
          protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                //Extract the forms authentication cookie
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                // If caching roles in userData field then extract
                string[] roles = authTicket.UserData.Split(new char[] { '|' });
                //roles = new string[] { "Reader" };

                // Create the IIdentity instance
                IIdentity id = new FormsIdentity(authTicket);

                // Create the IPrinciple instance
                IPrincipal principal = new GenericPrincipal(id, roles);

                // Set the context user 
                Context.User = principal;
            }
        }
  
HomeController.cs

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
                //FormsAuthentication.SetAuthCookie(user.Login, false);
                var roles = new List<string>();
                using (var con = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    var cmd = new System.Data.SqlClient.SqlCommand(@"
SELECT R.[RoleName] 
FROM [aspnet_Roles] R 
INNER JOIN aspnet_UsersInRoles UR ON UR.RoleId = R.RoleId 
INNER JOIN aspnet_Users U ON U.UserId = UR.UserId
WHERE U.UserName='" + user.Login + @"' ", con);
                    var dt = new System.Data.DataTable();
                    var sda = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    sda.Fill(dt);
                    foreach(System.Data.DataRow dr in dt.Rows)
                    {
                        roles.Add(dr["RoleName"].ToString());
                    }
                }
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

        public ActionResult BadPassword()
        {
            ViewBag.Message = "Bad password";

            return View();
        }

 
 reinitialiser le mot de passe :
             var membershipUser = Membership.GetUser();
            membershipUser.ChangePassword("tototiti", "tototutu");
 
  
  
  