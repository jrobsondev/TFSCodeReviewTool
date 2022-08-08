using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using TFSCodeReviewTool.Properties;
using TFSCodeReviewTool.ReportDataSources;
using TFSCodeReviewTool.ReportElements;
using TFSCodeReviewTool.Reports;

namespace TFSCodeReviewTool
{
    internal class ReportManager
    {
        private string _SaveLocation;
        private CodeReviewReport _CodeReviewReport;

        public ReportManager(string saveLocation)
        {
            _SaveLocation = saveLocation;
            _CodeReviewReport = new CodeReviewReport();
        }

        public void GenerateCodeReviewReportPDF(List<CodeReviewReportDataSource> dataSources) => _CodeReviewReport.GeneratePDF(dataSources, _SaveLocation);
    }
}
