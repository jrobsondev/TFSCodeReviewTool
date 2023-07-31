using Microsoft.TeamFoundation.Discussion.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TFSCodeReviewTool.Models
{
    public class CodeReview
    {
        public string ProjectName { get; }
        public int ReviewNumber { get; }
        public string ReviewForName { get; }
        public string ReviewByName { get; set; }
        public DateTime ReviewSentOnDateTime { get; private set; }
        public DateTime ReviewCompletedOnDateTime { get; private set; }
        public string OverallComment { get; set; }

        public CodeReview(IEnumerable<DiscussionThread> discussionThreads, string projectName, int reviewId, WorkItem workItem)
        {
            var reviewerComment = discussionThreads.OrderByDescending(x => x.PublishedDate).First(x => x.RootComment.Author.UniqueName != workItem.CreatedBy);
            ProjectName = projectName;
            ReviewNumber = reviewId;
            ReviewForName = workItem.CreatedBy;
            ReviewSentOnDateTime = workItem.CreatedDate;
            ReviewByName = reviewerComment.RootComment.Author.DisplayName;
            ReviewCompletedOnDateTime = reviewerComment.PublishedDate;
            OverallComment = workItem.Title;
        }
    }
}
