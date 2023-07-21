using System.Collections.Generic;
using TFSCodeReviewTool.ReportDataSources;
using TFSCodeReviewTool.Reports;

namespace TFSCodeReviewTool
{
    internal class ReportManager
    {
        private readonly string _SaveLocation;
        private readonly CodeReviewReport _CodeReviewReport;

        public ReportManager(string saveLocation)
        {
            _SaveLocation = saveLocation;
            _CodeReviewReport = new CodeReviewReport();
        }

        public void GenerateCodeReviewReportPDF(List<CodeReviewReportDataSource> dataSources) => _CodeReviewReport.GeneratePDF(dataSources, _SaveLocation);
    }
}
