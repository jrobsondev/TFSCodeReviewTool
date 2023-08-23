using CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TFSCodeReviewTool.Managers.Managers;
using TFSCodeReviewTool.ReportDataSources;

namespace TFSCodeReviewTool
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed((err) => err.Output());
        }

        static void RunOptions(Options opts)
        {
            var tfsManager = new TFSManager(opts.ProjectName);
            var saveLocationFilePath = string.IsNullOrEmpty(opts.SaveLocationFilePath) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : opts.SaveLocationFilePath;
            var reportManager = new ReportManager(saveLocationFilePath, opts.FileNameFormatString, opts.DateTimeFormatString);
            var reportDataSources = new List<CodeReviewReportDataSource>();

            foreach (var reviewId in opts.ReviewWorkItemIds)
            {
                var codeReviewData = tfsManager.GetCodeReviewComments(reviewId);
                if (codeReviewData != default) { reportDataSources.Add(new CodeReviewReportDataSource(codeReviewData)); }
            }

            if (opts.IndividualReports)
            {
                reportDataSources.ForEach(rds =>
                {
                    var filePath = reportManager.GenerateCodeReviewReportPDF(rds);
                    if (opts.AutoOpenPDF) { Process.Start(filePath); }
                });
            }
            else
            {
                var filePath = reportManager.GenerateCodeReviewReportPDF(reportDataSources);
                if (opts.AutoOpenPDF) { Process.Start(filePath); }
            }
        }
    }
}
