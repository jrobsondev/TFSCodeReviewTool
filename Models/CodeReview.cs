using Microsoft.TeamFoundation.Discussion.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            var shelvesetOwner = mainComment.VersionUri.Segments.Last().Split(new string[] { "%253d" }, StringSplitOptions.None).Last();
            shelvesetOwner = shelvesetOwner.Replace("%252540", "@");
            var reviewerComment = discussionThreads.OrderByDescending(x => x.PublishedDate).First(x => x.RootComment.Author.UniqueName != shelvesetOwner);
            ProjectName = projectName;
            ReviewNumber = reviewId;
            ReviewForChangeset = mainComment.VersionUri.Segments.Last();
            ReviewForName = ConvertEmailToName(shelvesetOwner);
            ReviewSentOnDateTime = mainComment.PublishedDate;
            ReviewByName = reviewerComment.RootComment.Author.DisplayName;
            ReviewCompletedOnDateTime = reviewerComment.PublishedDate;
            OverallComment = mainComment.RootComment?.Content;
        }

        private string ConvertEmailToName(string email)
        {
            var joinedName = email.Split('@').First();
            var firstAndLastName = joinedName.Split('.');
            var textInfo = new CultureInfo("en-GB", false).TextInfo;
            return textInfo.ToTitleCase(string.Join(" ", firstAndLastName));
        }
    }
}
