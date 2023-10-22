using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CustomLogin.Hubs;
using CustomLogin.Models;


namespace CustomLogin.Controllers
{
    public class UserAccountsController : Controller
    {
        private OurDbContext db = new OurDbContext();

        public ActionResult Login()
        {
            Session["UserID"] = 0;
            return View();
        }
        [HttpPost]
        public ActionResult Login(UserAccount user)
        {
 
                var usr = db.userAccount.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);
                if (usr != null)
                {
               
                    Session["UserID"] = usr.UserID.ToString();
                    Session["Username"] = usr.Username.ToString();
                  
                    return RedirectToAction("Details");
                }
                else
                {
                    ModelState.AddModelError("", "Username or password is wrong");
                }
            
            return View();
        }
  
        public ActionResult LoginAsAdmin(Admin admin)
        {

            var admin1 = db.admin.FirstOrDefault(u => u.Email == admin.Email && u.Password == admin.Password);
            if (admin1 != null)
            {

                Session["UserID"] = admin1.UserID.ToString();
                Session["Username"] = admin1.Email.ToString();

                return RedirectToAction("creatCourse", "course");
            }
            else
            {
                ModelState.AddModelError("", "Username or password is wrong");
            }

            return View();
        }


        public ActionResult Details()
        {
            int id = Convert.ToInt32(Session["UserID"]);
           
            return View(db.userAccount.Find(id));
        }

        // GET: UserAccounts/Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,FirstName,LastName,Email,Username,Password,ConfirmPassword,City,Country,mobile,image")] UserAccount userAccount,HttpPostedFileBase file)
        {

            if (file == null)
            {
                userAccount.image = "/image/defult.png";
            } 
            if (file != null)
            {
                string filename = Path.GetFileNameWithoutExtension(file.FileName);
                string extension = Path.GetExtension(file.FileName);
                filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                userAccount.image = "/image/" + filename;
                filename = Path.Combine(Server.MapPath("/image/"), filename);
                file.SaveAs(filename);
            }
           
            if (ModelState.IsValid)
            {
                db.userAccount.Add(userAccount);
                db.SaveChanges();
                Session["UserID"] = userAccount.UserID.ToString();
                return RedirectToAction("Details");
            }
           

            return View(userAccount);
        }
        public ActionResult CreateAdminAccount()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAdminAccount([Bind(Include = "UserID,Email,Username,Password")] Admin admin)
        {

      
            if (ModelState.IsValid)
            {
                db.admin.Add(admin);
                db.SaveChanges();
                Session["UserID"] = admin.UserID.ToString();
                return RedirectToAction("creatCourse", "course");
            }


            return View(admin);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAccount userAccount = db.userAccount.Find(id);
            if (userAccount == null)
            {
                return HttpNotFound();
            }
            return View(userAccount);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,FirstName,LastName,Email,Username,Password,ConfirmPassword,City,Country,mobile,image")] UserAccount userAccount, HttpPostedFileBase file)
        {
            
            if (file!=null){

                string filename = Path.GetFileNameWithoutExtension(file.FileName);
                string extension = Path.GetExtension(file.FileName);
                filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                userAccount.image = "/image/" + filename;
                filename = Path.Combine(Server.MapPath("~/image/"), filename);
                file.SaveAs(filename);
               
            }
            
            if (ModelState.IsValid)
            {
                db.Entry(userAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details");
            }
            return View(userAccount);
        }

        public JsonResult GetUser()
        {

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["userconnection"].ConnectionString))
            {
              
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT [UserID],[FirstName],[LastName],[Email],[Username],[City],[Country],[mobile],[image]FROM [dbo].[UserAccount] Where[UserID]=  " + Convert.ToInt32(Session["UserID"]), connection))
                {

                    command.Notification = null;

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChangeUser);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    var list = UserData(reader);

                    return list;

                }

            }
        }
        public JsonResult UserData(SqlDataReader reader)
        {
            var list = reader.Cast<IDataRecord>().Select(x => new
            {


                LastName = (string)x["LastName"],
                FirstName = (string)x["FirstName"],
                Email = (string)x["Email"],
                Username = (string)x["Username"],
                City = (string)x["City"],
                Country = (string)x["Country"],
                image = (string)x["image"],
                mobile = (int)x["mobile"],
                UserID = (int)x["UserID"],


            }).ToList();

            return Json(new { list = list }, JsonRequestBehavior.AllowGet);
        }
        private void dependency_OnChangeUser(object sender, SqlNotificationEventArgs e)
        {
            userHub1.Show();
        }
       
    
        public JsonResult GetUsers()
        {

                string searching = (string)Session["Email"];   
                    var list = db.userAccount.Where(x => x.Email.Contains(searching) || searching == null).Select(x => new
                    {

                        userId=(int)x.UserID,
                        Username = (string)x.Username,
                      
                    image = (string)x.image,
                       

                    }).ToList();

                    return Json(new { list = list }, JsonRequestBehavior.AllowGet);
        }
        private void dependency_OnChangeUsers(object sender, SqlNotificationEventArgs e)
        {
            searchHub.Show();
        }


        public ActionResult allstudents()
        {
            int id = Convert.ToInt32(Session["UserID"]);

            return View(db.userAccount.Find(id));
        }
   

        public JsonResult GetUseres()
        {

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["userconnection"].ConnectionString))
            {

                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT [UserID],[FirstName],[LastName],[Email],[Username],[City],[Country],[mobile],[image]FROM [dbo].[UserAccount]", connection))
                {

                    command.Notification = null;

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(Dependency_OnChangeUsers);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    var list = UseresData(reader);

                    return list;

                }

            }
        }
        public JsonResult UseresData(SqlDataReader reader)
        {
            var list = reader.Cast<IDataRecord>().Select(x => new
            {


                LastName = (string)x["LastName"],
                FirstName = (string)x["FirstName"],
                Email = (string)x["Email"],
                Username = (string)x["Username"],
                City = (string)x["City"],
                Country = (string)x["Country"],
                image = (string)x["image"],
                mobile = (int)x["mobile"],
                UserID = (int)x["UserID"],


            }).ToList();

            return Json(new { list = list }, JsonRequestBehavior.AllowGet);
        }

        private void Dependency_OnChangeUsers(object sender, SqlNotificationEventArgs e)
        {
            userHub1.Show();
        }

        public ActionResult delet(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAccount student = db.userAccount.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            var filteredCourses = db.sub.Where(sub => sub.studentId == id).ToList();

            // Step 2: Loop through the filtered records and remove them from the context.
            foreach (var sub in filteredCourses)
            {
                courses course = db.course.Find(sub.courseID);
                course.Capacity --;
                db.Entry(course).State = EntityState.Modified;
                db.sub.Remove(sub);
                db.SaveChanges();
            }

            db.userAccount.Remove(student);
            db.SaveChanges();
            return RedirectToAction("allstudents");
        }
  

    }


}

