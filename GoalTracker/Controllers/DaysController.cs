using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using GoalTracker.Models;

namespace GoalTracker.Controllers
{
    [Authorize]
    public class DaysController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Days
        public ActionResult Index(Guid? ClassId)
        {
            if (ClassId != null)
            {
                Session["ClassId"] = (Guid)ClassId;
            }

            if (Session["ClassId"] != null)
            {
                // Cannot use ClassId because its nullable and LINQ doesnt like that
                Guid cId = (Guid)Session["ClassId"];
                var ret = db.Days
                    .Where(g => g.ClassAssigned.ClassId.Equals(cId))
                    .ToList();
                return View(ret.OrderBy(d => d.Date));
            }

            return RedirectToAction("Index", "Classes");
        }

        // GET: Days/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Day day = db.Days.Find(id);
            if (day == null)
            {
                return HttpNotFound();
            }
            return View(day);
        }

        // GET: Days/Create
        [Authorize(Roles = "Instructor")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Days/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor")]
        public ActionResult Create([Bind(Include = "DayId,Date,DefaultGoal,IsDayActive")] Day day)
        {
            if (Session["ClassId"] != null)
            {
                Guid classId = (Guid)Session["ClassId"];
                if (db.Days.Any(d => d.Date.Equals(day.Date) && d.ClassAssigned.ClassId.Equals(classId)))
                {
                    ViewBag.Error = "Day already exists in this class with the same date.";
                    return View(day);
                }
                if (ModelState.IsValid)
                {
                    day.DayId = Guid.NewGuid();
                    day.ClassAssigned = db.Classes.FirstOrDefault(c => c.ClassId.Equals(classId));
                    db.Days.Add(day);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(day);
            }
            return RedirectToAction("Index", "Classes");
        }

        // GET: Days/Edit/5
        [Authorize(Roles = "Instructor")]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Day day = db.Days.Find(id);
            if (day == null)
            {
                return HttpNotFound();
            }
            return View(day);
        }

        // POST: Days/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor")]
        public ActionResult Edit([Bind(Include = "DayId,Date")] Day day)
        {
            if (ModelState.IsValid)
            {
                db.Entry(day).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(day);
        }

        // GET: Days/Delete/5
        [Authorize(Roles = "Instructor")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Day day = db.Days.Find(id);
            if (day == null)
            {
                return HttpNotFound();
            }
            return View(day);
        }

        // POST: Days/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Day day = db.Days.Find(id);
            db.Days.Remove(day);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
