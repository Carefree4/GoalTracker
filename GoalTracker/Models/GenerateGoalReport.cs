using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using OfficeOpenXml;

namespace GoalTracker.Models
{
    public class GenerateGoalReport : GenerateReport
    {
        public GenerateGoalReport(DataTable Data) 
            : base(Data)
        {
        }

        public override ExcelPackage GetReport()
        {
            return base.GetReport();
        }
    }
}