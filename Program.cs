using TFSCodeReviewTool.Managers.Managers;
using TFSCodeReviewTool.Properties;
using TFSCodeReviewTool.ReportDataSources;

namespace TFSCodeReviewTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var reviewId = 299; //Get from args
            var tfsManager = new TFSManager(Settings.Default.TFSProjectName);
            var codeReviewData = tfsManager.GetCodeReviewComments(reviewId);
            var reportDataSource = new CodeReviewReportDataSource(codeReviewData);
            var reportManager = new ReportManager($"D:\\Temp\\{reviewId}.pdf");
            reportManager.GenerateCodeReviewReportPDF(reportDataSource);
        }
    }
}
