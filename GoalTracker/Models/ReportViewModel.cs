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

    public class ReportedData
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public double ProfessionalInteractionPoints { get; set; }
        public double EffortScore { get; set; }
        public double TotalScore
        {
            get
            {
                return ProfessionalInteractionPoints + EffortScore;
            }
        }
    }
}