using Microsoft.TeamFoundation.Discussion.Client;
using System;
using System.Collections.Generic;
using TFSCodeReviewTool.Models;

namespace TFSCodeReviewTool.Managers.Managers
{
    internal class TFSManager
    {
        private string _ProjectName;
        private Uri _TFSServerAddress;

        public TFSManager(string projectName)
        {
            _ProjectName = projectName;
            _TFSServerAddress = GetProjectUrl();
        }

        public Tuple<CodeReview, List<CodeReviewComment>> GetCodeReviewComments(int reviewId)
        {
            List<CodeReviewComment> comments = new List<CodeReviewComment>();

            var projectURI = _TFSServerAddress;
            var service = new TeamFoundationDiscussionService();
            service.Initialize(new Microsoft.TeamFoundation.Client.TfsTeamProjectCollection(projectURI));
            IDiscussionManager discussionManager = service.CreateDiscussionManager();

            IAsyncResult result = discussionManager.BeginQueryByCodeReviewRequest(reviewId, QueryStoreOptions.ServerAndLocal, new AsyncCallback(CallCompletedCallback), null);
            var discussionThreads = discussionManager.EndQueryByCodeReviewRequest(result);

            CodeReview codeReview = new CodeReview(discussionThreads, reviewId, _ProjectName);

            foreach (var discussionThread in discussionThreads)
            {
                if (discussionThread.RootComment != null)
                {
                    comments.Add(new CodeReviewComment(discussionThread));
                }
            }

            return Tuple.Create(codeReview, comments);
        }

        private Uri GetProjectUrl()
        {
            const string BASE_AZURE_ADDRESS = "https://dev.azure.com/";
            Uri uri = new Uri(BASE_AZURE_ADDRESS);
            return new Uri(uri, _ProjectName);
        }

        static void CallCompletedCallback(IAsyncResult result) { }
    }
}
