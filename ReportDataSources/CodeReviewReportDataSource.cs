using System;
using System.Collections.Generic;
using TFSCodeReviewTool.Models;

namespace TFSCodeReviewTool.ReportDataSources
{
    internal class CodeReviewReportDataSource
    {
        public CodeReview CodeReview { get; }
        public DateTime ReviewGeneratedOnDateTime { get; private set; }
        public List<CodeReviewComment> CodeReviewComments { get; }

        public CodeReviewReportDataSource(Tuple<CodeReview, List<CodeReviewComment>> data)
        {
            CodeReview = data.Item1;
            ReviewGeneratedOnDateTime = DateTime.Now;
            CodeReviewComments = data.Item2;
        }
    }
}
