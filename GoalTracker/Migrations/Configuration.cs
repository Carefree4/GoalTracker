using GoalTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GoalTracker.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<GoalTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(GoalTracker.Models.ApplicationDbContext context)
        {
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            context.Roles.AddOrUpdate(r => r.Name, new IdentityRole { Name = "Admin" });
            context.Roles.AddOrUpdate(r => r.Name, new IdentityRole { Name = "Instructor" });
            context.Roles.AddOrUpdate(r => r.Name, new IdentityRole { Name = "Student" });

            if (!context.Users.Any(t => t.UserName == "maxdwenger@gmail.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "maxdwenger@gmail.com",
                    Email = "maxdwenger@gmail.com",
                };

                userManager.Create(user, "123Adm!n123");

                userManager.AddToRole(user.Id, "Admin");
            }

            if(!context.Users.Any(t => t.UserName == "teacher@nsd.org"))
            {
                var user = new ApplicationUser
                {
                    UserName = "teacher@nsd.org",
                    Email = "teacher@nsd.org",
                };

                userManager.Create(user, "123Teacher!123");

                userManager.AddToRole(user.Id, "Instructor");
            }

            if (!context.Users.Any(t => t.UserName == "student@nsd.org"))
            {
                var user = new ApplicationUser
                {
                    UserName = "student@nsd.org",
                    Email = "student@nsd.org",
                };

                userManager.Create(user, "123Student!123");

                userManager.AddToRole(user.Id, "Student");
            }

            context.SaveChanges();

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
