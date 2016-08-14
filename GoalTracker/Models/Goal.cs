using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalTracker.Models
{
    public class Goal
    {
        [Key]
        public Guid FormId { get; set; }

        [Display(Name = "Goal")]
        public string GoalText { get; set; }

        public string Accomplishment { get; set; }

        [Display(Name = "Effort Score")]
        public double EffortScore { get; set; }

        [Display(Name = "Professional Interaction Points")]
        public double ProfessionalInteractionPoints { get; set; }

        [InverseProperty("Goals")]
        public virtual ApplicationUser Student { get; set; }

        [InverseProperty("Goals")]
        public virtual Day DayOfGoal { get; set; }
    }
}