using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using GoalTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data;
using System.IO;

namespace GoalTracker.Controllers
{
    [Authorize]
    public class ClassesController : Controller
    {
        protected ApplicationDbContext Db { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public ClassesController()
        {
            this.Db = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.Db));

        }

        // GET: Classes
        public ActionResult Index()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());

            // Admins get to see all classes :)
            if (User.IsInRole("Admin"))
            {
                return View(Db.Classes.ToList());
            }

            if (User.IsInRole("Instructor"))
            {
                return View(user.ClassesInstructing.Concat(user.ClassesAttending).ToList());
            }

            // Get to see the classes you are in
            return View(user.ClassesAttending);
        }

        [Authorize(Roles = "Student")]
        public ActionResult JoinClass()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public ActionResult JoinClass([Bind(Include = "JoinId")] Class @class)
        {
            var result = Db.Classes.FirstOrDefault(c => c.JoinId.Equals(@class.JoinId));

            if (result != null)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());

                user.ClassesAttending.Add(result);
                Db.SaveChanges();
                return RedirectToAction("Index");
            }


            return View(@class);
        }

        // GET: Classes/Details/5
        [Authorize(Roles = "Instructor")]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = Db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            return View(@class);
        }

        public ActionResult ClassHome(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = Db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            return View(@class);
        }

        // GET: Classes/Create
        [Authorize(Roles = "Instructor")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Classes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor")]
        public ActionResult Create([Bind(Include = "ClassId,ClassName,ClassDescription")] Class @class)
        {
            if (ModelState.IsValid)
            {
                @class.ClassId = Guid.NewGuid();
                @class.JoinId = Class.NewJoinId();
                @class.DefaultDaysOfWeek = Weekdays.WorkDays;

                var user = UserManager.FindById(User.Identity.GetUserId());
                user.ClassesInstructing.Add(@class);

                Db.Classes.Add(@class);
                Db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(@class);
        }

        // GET: Classes/Edit/5
        [Authorize(Roles = "Instructor")]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = Db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            return View(@class);
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor")]
        public ActionResult Edit([Bind(Include = "ClassId,JoinId,ClassName,DefaultDaysOfWeek,ClassDescription")] Class @class)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(@class).State = EntityState.Modified;
                Db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(@class);
        }

        // GET: Classes/Delete/5
        [Authorize(Roles = "Instructor")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = Db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            return View(@class);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Class @class = Db.Classes.Find(id);
            Db.Classes.Remove(@class);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Instructor")]
        public ActionResult ManageStudents(Guid id)
        {
            // If this goes big, this NEEDS to be fixed. Such bad code ;_;
            ViewBag.ClassId = (Guid)id;
            var students = Db.Users.Where(u => u.ClassesAttending.Any(c => c.ClassId.Equals(id)));
            return View(students);
        }

        [Authorize(Roles = "Instructor")]
        public ActionResult KickStudentFromClass(Guid studentId, Guid classId)
        {
            var student = Db.Users.FirstOrDefault(u => u.Id.Equals(studentId.ToString()));
            var classToRemove = Db.Classes.FirstOrDefault(c => c.ClassId.Equals(classId));
            student.ClassesAttending.Remove(classToRemove);
            Db.SaveChanges();

            return RedirectToAction("ManageStudents", new { id = classId });
        }

        [Authorize(Roles = "Instructor")]
        public ActionResult Report(Guid? ClassId)
        {
            var report = new ReportViewModel()
            {
                ReportedClassId = Db.Classes.FirstOrDefault(c => c.ClassId == ClassId).ClassId,
                StartDay = DateTime.Today,
                EndDay = DateTime.Today
            };

            if (report.ReportedClassId == null)
            {
                RedirectToAction("Index");
            }

            return View(report);
        }

        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public ActionResult Report(ReportViewModel report)
        {
            var xlReportFile = new Models.GenerateGoalReport(GetAllStudentGoals(report)).GetReport();

            // Prepare file
            var filename = report.StartDay + "-" + report.EndDay + ".xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            // Create File Stream and set prefrences
            var fileStream = new MemoryStream();
            xlReportFile.SaveAs(fileStream);
            fileStream.Position = 0;

            var fsr = new FileStreamResult(fileStream, contentType);
            fsr.FileDownloadName = filename;

            return fsr;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Helpers
        private DataTable GetAllStudentGoals(ReportViewModel report)
        {
            // Get all goals for the class within the set date
            var Goals = Db.Goals.Where(g =>
                                            g.DayOfGoal.Date >= report.StartDay
                                            && g.DayOfGoal.Date <= report.EndDay
                                            && g.DayOfGoal.ClassAssigned.ClassId.Equals(report.ReportedClassId)).OrderBy(g => g.Student.Id).ToList();

            var data = InitGoalDataTable();

            // Add all the points togeather for each student and insert one
            // row per student.
            ReportedData lastGoal = null;
            foreach (Goal g in Goals)
            {
                if (lastGoal == null)
                {
                    lastGoal = new ReportedData
                    {
                        Id = g.Student.Id,
                        LastName = g.Student.LastName,
                        FirstName = g.Student.FirstName,
                        Email = g.Student.Email
                    };
                }

                if (g.Student.Id.Equals(lastGoal.Id))
                {
                    lastGoal.ProfessionalInteractionPoints += g.ProfessionalInteractionPoints;
                    lastGoal.EffortScore += g.EffortScore;
                }
                else
                {
                    NewGoalDataTableRow(data, lastGoal);
                    lastGoal = null;
                }
            }

            // G'D Fencepost
            NewGoalDataTableRow(data, lastGoal);

            return data;
        }

        private static void NewGoalDataTableRow(DataTable data, ReportedData lastGoal)
        {
            data.Rows.Add(
                lastGoal.LastName,
                lastGoal.FirstName,
                lastGoal.Email,
                lastGoal.ProfessionalInteractionPoints,
                lastGoal.EffortScore,
                lastGoal.TotalScore
                );
        }

        private static DataTable InitGoalDataTable() {
            var data = new DataTable();
            data.Columns.Add("Last Name", typeof(string));
            data.Columns.Add("First Name", typeof(string));
            data.Columns.Add("Email", typeof(string));
            data.Columns.Add("PIP Points", typeof(int));
            data.Columns.Add("Effort Score", typeof(int));
            data.Columns.Add("Total", typeof(int));

            return data;
        }
        #endregion

    }
}
