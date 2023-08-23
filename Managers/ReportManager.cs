using System.Collections.Generic;
using TFSCodeReviewTool.ReportDataSources;
using TFSCodeReviewTool.Reports;

namespace TFSCodeReviewTool
{
    internal class ReportManager
    {
        private readonly string _SaveLocationFilePath;
        private readonly string _FileNameFormatString;
        private readonly string _DateTimeFormatString;
        private readonly CodeReviewReport _CodeReviewReport;

        public ReportManager(string saveLocationFilePath, string fileNameFormatString, string dateTimeFormatString)
        {
            _SaveLocationFilePath = saveLocationFilePath;
            _FileNameFormatString = fileNameFormatString;
            _DateTimeFormatString = dateTimeFormatString;
            _CodeReviewReport = new CodeReviewReport();
        }

        public string GenerateCodeReviewReportPDF(CodeReviewReportDataSource dataSource) => _CodeReviewReport.GeneratePDF(dataSource, _SaveLocationFilePath, _FileNameFormatString, _DateTimeFormatString);
        public string GenerateCodeReviewReportPDF(List<CodeReviewReportDataSource> dataSources) => _CodeReviewReport.GenerateMergedPDF(dataSources, _SaveLocationFilePath, _FileNameFormatString, _DateTimeFormatString);
    }
}
