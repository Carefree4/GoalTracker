using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using GoalTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
