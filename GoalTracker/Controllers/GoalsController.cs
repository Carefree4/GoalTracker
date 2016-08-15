using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using GoalTracker.Models;
using Microsoft.AspNet.Identity;

namespace GoalTracker.Controllers
{
    [Authorize]
    public class GoalsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: Goals
        public ActionResult Index(Guid? ClassId)
        {

            return RedirectToAction("Index", "Days");

//            if (ClassId != null)
//            {
//                Session["ClassId"] = (Guid)ClassId;
//            }
//
//            if (Session["ClassId"] != null)
//            {
//                // Cannot use ClassId because its nullable and LINQ doesnt like that
//                Guid cId = (Guid)Session["ClassId"];
//                if (User.IsInRole("Instructor"))
//                {
//                    return View(db.Goals.Where(g => g.DayOfGoal.ClassAssigned.ClassId.Equals(cId)).ToList());
//                }
//                string userId = User.Identity.GetUserId();
//                return View(db.Goals.Where(
//                    g => g.DayOfGoal.ClassAssigned.ClassId.Equals(cId)
//                    && g.Student.Id.Equals(userId))
//                    .ToList());
//            }
//
//            return RedirectToAction("Index", "Classes");
        }

        // GET: Goals/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Goal goal = db.Goals.Find(id);
            if (goal == null)
            {
                return HttpNotFound();
            }
            return View(goal);
        }

        // GET: Goals/Create
        public ActionResult Create(Guid DayId)
        {
            Session["DayId"] = DayId;
            
            var day = db.Days.FirstOrDefault(d => d.DayId.Equals(DayId));
            var userId = User.Identity.GetUserId();
            var goal = db.Goals.FirstOrDefault(g => g.DayOfGoal.DayId.Equals(day.DayId) && g.Student.Id.Equals(userId));
            if (goal == null)
            {
                goal = new Goal {GoalText = day.DefaultGoal};
            }
            else
            {
                return RedirectToAction("Edit", new { id = goal.FormId });
            }

            return View(goal);
        }

        // POST: Goals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FormId,GoalText,Accomplishment,EffortScore")] Goal goal)
        {
            if (!ModelState.IsValid) return View(goal);
            if (Session["DayId"] == null) return View(goal);

            var dayId = (Guid)Session["DayId"];
            var userId = User.Identity.GetUserId();

            goal.FormId = Guid.NewGuid();
            goal.DayOfGoal = db.Days.FirstOrDefault(d => d.DayId.Equals(dayId));
            goal.Student = db.Users.FirstOrDefault(u => u.Id.Equals(userId));
            db.Goals.Add(goal);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Goals/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Goal goal = db.Goals.Find(id);
            if (goal == null)
            {
                return HttpNotFound();
            }
            return View(goal);
        }

        // POST: Goals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FormId,GoalText,Accomplishment,EffortScore,ProfessionalInteractionPoints")] Goal goal)
        {
            if (ModelState.IsValid)
            {
                db.Entry(goal).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(goal);
        }

        // GET: Goals/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Goal goal = db.Goals.Find(id);
            if (goal == null)
            {
                return HttpNotFound();
            }
            return View(goal);
        }

        // POST: Goals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Goal goal = db.Goals.Find(id);
            db.Goals.Remove(goal);
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
