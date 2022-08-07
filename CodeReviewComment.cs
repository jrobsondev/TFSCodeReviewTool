using Microsoft.TeamFoundation.Discussion.Client;
using System.IO;

namespace TFSCodeReviewTool
{
    public class CodeReviewComment
    {
        public string Comment { get; }
        public string FileName { get; }
        public int? StartLineNumber { get; private set; }
        public int? EndLineNumber { get; private set; }
        public string LineSpanText { get; }

        public CodeReviewComment(DiscussionThread discussionThread)
        {
            Comment = discussionThread.RootComment.Content;
            FileName = Path.GetFileName(discussionThread.ItemPath);
            StartLineNumber = discussionThread.Position?.StartLine;
            EndLineNumber = discussionThread.Position?.EndLine;
            LineSpanText = GetLineSpanText();
        }

        private string GetLineSpanText()
        {
            if (StartLineNumber.HasValue)
            {
                if (StartLineNumber == EndLineNumber)
                {
                    return $"Line: {StartLineNumber}";
                }
                else
                {
                    return $"Lines {StartLineNumber} to {EndLineNumber}";
                }
            }
            return string.Empty;
        }
    }
}
