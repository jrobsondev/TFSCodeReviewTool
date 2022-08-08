using Microsoft.TeamFoundation.Discussion.Client;
using System.IO;

namespace TFSCodeReviewTool.Models
{
    public class CodeReviewComment
    {
        public string Comment { get; }
        public string FileName { get; }
        public int? StartLineNumber { get; private set; }
        public int? EndLineNumber { get; private set; }
        public string LineSpanText { get; }
        public bool IsMainComment { get; set; }

        public CodeReviewComment(DiscussionThread discussionThread)
        {
            Comment = discussionThread.RootComment.Content;
            FileName = Path.GetFileName(discussionThread.ItemPath);
            StartLineNumber = discussionThread.Position?.StartLine;
            EndLineNumber = discussionThread.Position?.EndLine;
            LineSpanText = GetLineSpanText();
            IsMainComment = string.IsNullOrEmpty(FileName);
        }

        private string GetLineSpanText()
        {
            if (StartLineNumber.HasValue)
            {
                if (EndLineNumber == StartLineNumber + 1)
                {
                    return $"{StartLineNumber}";
                }
                else
                {
                    return $"{StartLineNumber} - {EndLineNumber}";
                }
            }
            return string.Empty;
        }
    }
}
