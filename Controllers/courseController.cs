using CustomLogin.Hubs;
using CustomLogin.Models;
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

namespace CustomLogin.Controllers
{
    public class courseController : Controller
    {
       
        private OurDbContext db = new OurDbContext();

       
        public ActionResult creatCourse()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult creatCourse([Bind(Include = "courseID,courseName,startDate,endDate,cost,Capacity,Status,CourseSyllabus")] courses course, HttpPostedFileBase file)
        {
            if (file == null)
            {
                course.CourseSyllabus = "/Course syllabus/christie_24_2.pdf";
            }
            if (file != null)
            {
                string filename = Path.GetFileNameWithoutExtension(file.FileName);
                string extension = Path.GetExtension(file.FileName);
                filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                course.CourseSyllabus  = "/Course syllabus/" + filename;
                filename = Path.Combine(Server.MapPath("/Course syllabus/"), filename);
                file.SaveAs(filename);
            }

            if (ModelState.IsValid)
            {
                course.Capacity = 0;
                db.course.Add(course);
                db.SaveChanges();
                return RedirectToAction("creatCourse");
            }

            return View(course);
        }
        public ActionResult courses()
        {
            int id = Convert.ToInt32(Session["UserID"]);


            return View(db.userAccount.Find(id));
        }

        public ActionResult usercourses()
        {
            return View();
        }
    
        public ActionResult editCourse(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            courses course = db.course.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editCourse([Bind(Include = "courseID,courseName,startDate,endDate,cost,Capacity,Status,CourseSyllabus")] courses course, HttpPostedFileBase file)
        {

            if (file != null)
            {
                string filename = Path.GetFileNameWithoutExtension(file.FileName);
                string extension = Path.GetExtension(file.FileName);
                filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                course.CourseSyllabus = "/Course syllabus/" + filename;
                filename = Path.Combine(Server.MapPath("/Course syllabus/"), filename);
                file.SaveAs(filename);
            }

            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("creatCourse");
            }
            return View(course);
        }
        public ActionResult delet(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            courses course = db.course.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            var filteredCourses = db.sub.Where(sub => sub.courseID == id).ToList();

            // Step 2: Loop through the filtered records and remove them from the context.
            foreach (var sub in filteredCourses)
            {
                db.sub.Remove(sub);
            }
            db.course.Remove(course);
            db.SaveChanges();
            return RedirectToAction("creatCourse");
        }
        public ActionResult subs(int id)
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            var filteredCourses = db.sub
                .Where(sub => sub.courseID == id && sub.studentId == userId)
                .ToList();
            if (!filteredCourses.Any()) {
                student_course su = new student_course();
                su.courseID = id;
                su.studentId = userId;
                courses course = db.course.Find(id);
                UserAccount student = db.userAccount.Find(userId);
                su.student = student;
                su.course = course;
                db.sub.Add(su);
                db.SaveChanges();
                var filteredStudent = db.sub
               .Where(sub => sub.courseID == id )
               .ToList();
                course.Capacity= filteredStudent.Count;
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
            }
        
            return RedirectToAction("Courses");

        }
        public ActionResult subscibedStudents()
        {
            int x = Convert.ToInt32(Session["UserID"]);

            return View(db.sub.Where(m => m.studentId == x).ToList());
        }
        public JsonResult GetSubscibedStudent()
        {

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["userconnection"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT [courseID] FROM [dbo].[student_course] WHERE  [studentId]=" +
               Convert.ToInt32(Session["UserID"]), connection))
                {
                    command.Notification = null;
                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChangefriends);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    var list = GetSubscibedStudents(reader);
                    return list;
                }
            }
        }
        public JsonResult GetSubscibedStudents(SqlDataReader reader)
        {
            var list = reader.Cast<IDataRecord>().Select(x => new
            {
            
                Username = db.course.Find((int)x["courseID"]).courseName,
                CourseSyllabus = db.course.Find((int)x["courseID"]).CourseSyllabus,
            }).ToList();

            return Json(new { list = list }, JsonRequestBehavior.AllowGet);
        }
        private void dependency_OnChangefriends(object sender, SqlNotificationEventArgs e)
        {
            friendsHub.Show();
        }
        public ActionResult Public (FormCollection form)
        {
            var course = db.course.Find(Convert.ToInt32(form["postid"]));
            if (ModelState.IsValid)
            {

         
                db.Entry(course).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("userPosts");
            }

            return RedirectToAction("userPosts");
        }
        public JsonResult Getcourse()
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["userconnection"].ConnectionString))
            {

                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT[courseName],[courseID],[startDate],[endDate],[cost],[Capacity],[Status],[CourseSyllabus] FROM [dbo].[courses]", connection))
                {


                    command.Notification = null;

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    var courselist = courseslist(reader);
                    return courselist;

                }
            }
        }
         public JsonResult courseslist(SqlDataReader reader)
                {

            
                    var courselist = reader.Cast<IDataRecord>().Select(x => new
                    {
                        courseID = (int)x["courseID"],
                        courseName = (string)x["courseName"],
                        startDate = (System.DateTime)x["startDate"],
                        endDate = (System.DateTime)x["endDate"],
                        cost = (int)x["cost"],
                        Capacity = (int)x["Capacity"],
                        Status = (string)x["Status"],
                        courseSyllaby = (string)x["CourseSyllabus"],

                    }).ToList();

                    return Json(new { courselist = courselist }, JsonRequestBehavior.AllowGet);

                
            
                }
         private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
            {
                userHub1.Show();
            }
    
       


       
    }
    }

    


