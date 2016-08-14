using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalTracker.Models
{
    public class Day
    {
        [Key]
        public Guid DayId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public string DefaultGoal { get; set; }

        public bool IsDayActive { get; set; }

        [Display(Name = "Class")]
        [InverseProperty("Days")]
        public virtual Class ClassAssigned { get; set; }

        [InverseProperty("DayOfGoal")]
        public virtual List<Goal> Goals { get; set; }
    }
}