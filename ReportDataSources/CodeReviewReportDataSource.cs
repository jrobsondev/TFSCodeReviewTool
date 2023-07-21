using System;
using System.Collections.Generic;
using TFSCodeReviewTool.Models;

namespace TFSCodeReviewTool.ReportDataSources
{
    internal class CodeReviewReportDataSource
    {
        public CodeReview CodeReview { get; }
        public List<CodeReviewComment> CodeReviewComments { get; }
        public DateTime ReviewGeneratedOnDateTime { get; private set; }

        public CodeReviewReportDataSource((CodeReview Review, List<CodeReviewComment> Comments) data)
        {
            CodeReview = data.Review;
            CodeReviewComments = data.Comments;
            ReviewGeneratedOnDateTime = DateTime.Now;
        }
    }
}
