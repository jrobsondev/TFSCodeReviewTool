using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Discussion.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using TFSCodeReviewTool.Models;

namespace TFSCodeReviewTool.Managers.Managers
{
    internal class TFSManager
    {
        private const string BASE_AZURE_ADDRESS = "https://dev.azure.com/";
        private readonly string _ProjectName;
        private readonly Uri _TFSServerAddress;

        public TFSManager(string projectName)
        {
            _ProjectName = projectName;
            _TFSServerAddress = GetProjectUrl();
        }

        public (CodeReview, List<CodeReviewComment>) GetCodeReviewComments(int reviewId)
        {
            var comments = new List<CodeReviewComment>();
            var projectURI = _TFSServerAddress;
            var tpc = new TfsTeamProjectCollection(projectURI);
            try
            {
                tpc.EnsureAuthenticated();
            }
            //If there are stale creds then we need to delete a registry key because it isn't handled by MS
            catch (TeamFoundationServerUnauthorizedException ex)
            {
                Registry.CurrentUser.DeleteSubKeyTree(@"Software\Microsoft\VSCommon\14.0\ClientServices\TokenStorage\VisualStudio\VssApp", false);
                tpc = new TfsTeamProjectCollection(projectURI);
                tpc.Authenticate();
            }
            var discussionService = new TeamFoundationDiscussionService();
            discussionService.Initialize(tpc);
            var discussionManager = discussionService.CreateDiscussionManager();

            var projectURI = _TFSServerAddress;
            var service = new TeamFoundationDiscussionService();
            service.Initialize(new Microsoft.TeamFoundation.Client.TfsTeamProjectCollection(projectURI));
            IDiscussionManager discussionManager = service.CreateDiscussionManager();

            IAsyncResult result = discussionManager.BeginQueryByCodeReviewRequest(reviewId, QueryStoreOptions.ServerAndLocal, new AsyncCallback(CallCompletedCallback), null);
            var result = discussionManager.BeginQueryByCodeReviewRequest(reviewId, QueryStoreOptions.ServerAndLocal, new AsyncCallback(CallCompletedCallback), null);
            var discussionThreads = discussionManager.EndQueryByCodeReviewRequest(result);

            if (discussionThreads.Count() > 1)
            {
                var codeReview = new CodeReview(discussionThreads, reviewId, _ProjectName);

                foreach (var discussionThread in discussionThreads)
                {
                    if (discussionThread.RootComment != null && discussionThread.RootComment.Id != -1)
                    {
                        comments.Add(new CodeReviewComment(discussionThread));
                    }
                }
                return (codeReview, comments);
            }
            return default;
        }

        private Uri GetProjectUrl()
        {
            var uri = new Uri(BASE_AZURE_ADDRESS);
            Uri uri = new Uri(BASE_AZURE_ADDRESS);
            return new Uri(uri, _ProjectName);
        }

        static void CallCompletedCallback(IAsyncResult result) { }
    }
}
