using Microsoft.TeamFoundation.Discussion.Client;
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
        public string ReviewForChangeset { get; }
        public string OverallComment { get; set; }

        public CodeReview(IEnumerable<DiscussionThread> discussionThreads, int reviewId, string projectName)
        {
            var mainComment = discussionThreads.First(x => string.IsNullOrEmpty(x.ItemPath));
            var reviewerComment = discussionThreads.OrderByDescending(x => x.PublishedDate).First(x => x.RootComment.Author.UniqueName != mainComment.RootComment.Author.UniqueName);
            ProjectName = projectName;
            ReviewNumber = reviewId;
            ReviewForChangeset = mainComment.VersionUri.Segments.Last();
            ReviewForName = mainComment.RootComment.Author.DisplayName;
            ReviewSentOnDateTime = mainComment.PublishedDate;
            ReviewByName = reviewerComment.RootComment.Author.DisplayName;
            ReviewCompletedOnDateTime = reviewerComment.PublishedDate;
            OverallComment = mainComment.RootComment.Content;
        }
    }
}
