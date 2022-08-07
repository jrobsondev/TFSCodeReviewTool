using Microsoft.TeamFoundation.Discussion.Client;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using System;
using System.Collections.Generic;
using TFSCodeReviewTool.Properties;

namespace TFSCodeReviewTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var reviewId = 299; //Get from args
            var codeReviewData = GetCodeReviewComments(reviewId);
            var reportDataSource = new CodeReviewReportDataSource(codeReviewData);
            CreatePDF(reportDataSource);
        }

        public static Tuple<CodeReview, List<CodeReviewComment>> GetCodeReviewComments(int reviewId)
        {
            List<CodeReviewComment> comments = new List<CodeReviewComment>();

            var projectURI = GetProjectUrl();
            var service = new TeamFoundationDiscussionService();
            service.Initialize(new Microsoft.TeamFoundation.Client.TfsTeamProjectCollection(projectURI));
            IDiscussionManager discussionManager = service.CreateDiscussionManager();

            IAsyncResult result = discussionManager.BeginQueryByCodeReviewRequest(reviewId, QueryStoreOptions.ServerAndLocal, new AsyncCallback(CallCompletedCallback), null);
            var discussionThreads = discussionManager.EndQueryByCodeReviewRequest(result);

            CodeReview codeReview = new CodeReview(discussionThreads, reviewId);

            foreach (var discussionThread in discussionThreads)
            {
                if (discussionThread.RootComment != null)
                {
                    comments.Add(new CodeReviewComment(discussionThread));
                }
            }

            return Tuple.Create(codeReview, comments);
        }

        static void CreatePDF(CodeReviewReportDataSource dataSource)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text(dataSource.CodeReview.ProjectName.ToUpperInvariant())
                        .SemiBold().FontSize(20);
                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Row(x =>
                        {
                            x.Spacing(20);
                            x.RelativeItem(1).Column(y =>
                            {
                                y.Item().Text($"Code Review No.: {dataSource.CodeReview.ReviewNumber}");
                                y.Item().Text($"Associated Changeset: {dataSource.CodeReview.ReviewForChangeset}");
                                y.Item().Text($"Review Created By: {dataSource.CodeReview.ReviewForName}");
                                y.Item().Text($"Review Created On: {dataSource.CodeReview.ReviewSentOnDateTime}");
                                y.Item().Text($"Reviewed By: {dataSource.CodeReview.ReviewByName}");
                                y.Item().Text($"Reviewed Completed On: {dataSource.CodeReview.ReviewCompletedOnDateTime}");
                            });
                            x.RelativeItem(1).Padding(5).Border(1).Column(y =>
                            {
                                y.Item().Border(1).Padding(2).Text("Overall Comments").Bold().FontColor(Colors.White).BackgroundColor(Colors.Grey.Medium);
                                y.Item().Padding(2).Text(dataSource.CodeReview.OverallComment);
                            });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            }).GeneratePdf("D:\\Temp\\test.pdf");
        }

        private static Uri GetProjectUrl()
        {
            const string BASE_AZURE_ADDRESS = "https://dev.azure.com/";
            string project = Settings.Default.TFSProjectName;
            Uri uri = new Uri(BASE_AZURE_ADDRESS);
            return new Uri(uri, project);
        }

        static void CallCompletedCallback(IAsyncResult result) { }
    }

   
}
