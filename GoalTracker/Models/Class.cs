using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalTracker.Models
{

    [Flags]
    public enum Weekdays
    {
        None = 0,
        Sunday = 1 << 0,
        Monday = 1 << 1,
        Tuesday = 1 << 2,
        Wednesday = 1 << 3,
        Thursday = 1 << 4,
        Friday = 1 << 5,
        Saturday = 1 << 6,
        WorkDays = Monday | Tuesday | Wednesday | Thursday | Friday
    }

    public class Class
    {

        public Class()
        {
            this.Days = new List<Day>();
            this.ClassAttendees = new List<ApplicationUser>();
            this.ClassInstructors = new List<ApplicationUser>();
        }

        [Key]
        public Guid ClassId { get; set; }

        [Display(Name = "Join Code")]
        public string JoinId { get; set; }

        public static string NewJoinId()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }

        [Required]
        [Display(Name = "Class Name")]
        public string ClassName { get; set; }

        // Default days the goals are required
        [Display(Name = "Default Days")]
        public Weekdays DefaultDaysOfWeek { get; set; }

        [Display(Name = "Class Description")]
        public string ClassDescription { get; set; }

        // Only Students
        [Display(Name = "Class Attendees")]
        [InverseProperty("ClassesAttending")]
        public  ICollection<ApplicationUser> ClassAttendees { get; set; }
        
        [Display(Name = "Class Instructors")]
        [InverseProperty("ClassesInstructing")]
        public ICollection<ApplicationUser> ClassInstructors { get; set; }

        [InverseProperty("ClassAssigned")]
        public ICollection<Day> Days { get; set; }
    }
}