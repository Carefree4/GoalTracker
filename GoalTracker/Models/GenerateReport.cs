using OfficeOpenXml;
using System;
using System.Data;

namespace GoalTracker.Models
{
    public class GenerateReport
    {
        private DataTable Data { get; set; }
        private ExcelPackage ExcelPackage { get; set; }
        protected ExcelWorksheet WorkSheet { get; set; }


        public GenerateReport(DataTable Data)
        {
            this.Data = Data;
        }


        public virtual ExcelPackage GetReport()
        {
            InitPackage();
            LoadDataIntoWorkBook();
            return ExcelPackage;
        }
        private ExcelPackage InitPackage()
        {
            ExcelPackage = new ExcelPackage();
            WorkSheet = ExcelPackage.Workbook.Worksheets.Add("Report");

            return ExcelPackage;

        }

        private ExcelWorksheet LoadDataIntoWorkBook()
        {
            WorkSheet.Cells["A1"].LoadFromDataTable(Data, true);

            return WorkSheet;
        }
    }
}