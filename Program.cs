using System;
using System.Collections.Generic;
using TFSCodeReviewTool.Managers.Managers;
using TFSCodeReviewTool.Properties;
using TFSCodeReviewTool.ReportDataSources;

namespace TFSCodeReviewTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var reviewIds = new List<int>() { 291, 293, 295, 297, 299, 301, 302 }; //Get from args
            var projectName = Settings.Default.TFSProjectName;
            var tfsManager = new TFSManager(projectName);
            var reportDataSources = new List<CodeReviewReportDataSource>();
            foreach (var reviewId in reviewIds)
            {
                var codeReviewData = tfsManager.GetCodeReviewComments(reviewId);
                if (codeReviewData != default) { reportDataSources.Add(new CodeReviewReportDataSource(codeReviewData)); }
            }
            var reportManager = new ReportManager($"D:\\Temp\\{projectName}-CodeReviewReport-{DateTime.Now:yyyy-MM-dd-HH-mm}.pdf"); //get from args or put in settings
            reportManager.GenerateCodeReviewReportPDF(reportDataSources);
        }
    }
}
