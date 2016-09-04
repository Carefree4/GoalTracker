using System;
using System.ComponentModel.DataAnnotations;

namespace GoalTracker.Models
{
    public class ReportViewModel
    {
        public Guid ReportedClassId { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDay { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDay { get; set; }
    }
}