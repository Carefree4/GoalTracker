using System;

namespace GoalTracker.Models
{
    public class ReportViewModel
    {
        public Class ReportedClass { get; set; }
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }
    }
}