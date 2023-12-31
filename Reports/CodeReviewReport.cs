﻿using Microsoft.VisualStudio.Services.Common;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TFSCodeReviewTool.ReportDataSources;
using TFSCodeReviewTool.ReportElements;

namespace TFSCodeReviewTool.Reports
{
    internal class CodeReviewReport
    {
        public string GeneratePDF(CodeReviewReportDataSource dataSource, string saveLocationFilePath, string fileNameFormatString, string dateTimeFormat)
        {
            var filePath = CreateSaveFilePath(saveLocationFilePath, fileNameFormatString, dataSource.CodeReview.ProjectName, dataSource.CodeReview.ReviewNumber.ToString(), dateTimeFormat);
            Document.Create(container => CreatePDFBody(container, dataSource)).GeneratePdf(filePath);
            return filePath;
        }

        public string GenerateMergedPDF(List<CodeReviewReportDataSource> dataSources, string saveLocationFilePath, string fileNameFormatString, string dateTimeFormat)
        {
            var dataSourceForTitle = dataSources.FirstOrDefault();
            var filePath = CreateSaveFilePath(saveLocationFilePath, fileNameFormatString, dataSourceForTitle.CodeReview.ProjectName, "MergedReport", dateTimeFormat);
            Document.Create(container => dataSources.ForEach(ds => CreatePDFBody(container, ds))).GeneratePdf(filePath);
            return filePath;
        }

        private string CreateSaveFilePath(string saveLocationFilePath, string fileNameFormatString, string projectName, string reviewNumber, string dateTimeFormat)
        {
            var filePath = Path.ChangeExtension(Path.Combine(saveLocationFilePath, string.Format(fileNameFormatString, projectName, reviewNumber, DateTime.Now.ToString(dateTimeFormat, new DateTimeFormatInfo { DateSeparator = "-" }))), ".pdf");
            if (!Directory.Exists(saveLocationFilePath))
            {
                Console.WriteLine($"Creating new directory: {saveLocationFilePath}");
                try { Directory.CreateDirectory(saveLocationFilePath); }
                catch (Exception ex) { throw new Exception($"Error: Unable to create directory ({ex.Message}).\nExiting."); }
            }
            return filePath;
        }

        public void CreatePDFBody(IDocumentContainer container, CodeReviewReportDataSource dataSource)
        {
            var codeReview = dataSource.CodeReview;
            var groupedComments = dataSource.CodeReviewComments.Where(x => x.FileName != null).GroupBy(x => x.FileName);
            var mainComments = dataSource.CodeReviewComments.Where(x => x.IsMainComment);
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .AlignCenter()
                    .Text($"Project: {codeReview.ProjectName} - Code Review: {codeReview.ReviewNumber}")
                    .SemiBold().FontSize(25).Underline();

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Grid(grid =>
                    {
                        grid.VerticalSpacing(1, Unit.Centimetre);
                        grid.Item(6).Grid(row =>
                        {
                            row.Item(4).Text("Review Created By:").Bold(); row.Item(6).Text(codeReview.ReviewForName);
                            row.Item(4).Text("Review Created On:").Bold(); row.Item(6).Text(codeReview.ReviewSentOnDateTime);
                            row.Item(4).Text("Reviewed By:").Bold(); row.Item(6).Text(codeReview.ReviewByName);
                            row.Item(4).Text("Reviewed On:").Bold(); row.Item(6).Text(codeReview.ReviewCompletedOnDateTime);
                        });

                        grid.Item(6).Border(1, Unit.Point).Grid(row =>
                        {
                            row.Item(12).Background(Colors.Grey.Lighten3).Border(1, Unit.Point).PaddingLeft(5).Text("Reviewee Comment").Bold();
                            row.Item(12).Padding(2).Text(codeReview.OverallComment).LineHeight(1);
                        });

                        grid.Item(12).Border(1, Unit.Point).Table(table =>
                        {
                            uint rowIndex = 1;
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.ConstantColumn(60, Unit.Point);
                                columns.RelativeColumn(2);
                            });

                            table.Cell().Row(rowIndex).Column(1).Element(Table.Header).Text("File").Bold();
                            table.Cell().Row(rowIndex).Column(2).Element(Table.Header).Text("Line").Bold();
                            table.Cell().Row(rowIndex).Column(3).Element(Table.Header).Text("Comment").Bold();

                            rowIndex++;

                            if (mainComments.Any())
                            {
                                table.Cell().Row(rowIndex).Column(1).RowSpan((uint)mainComments.Count()).Border(1, Unit.Point).PaddingLeft(5).Text("Overall Comments");
                                mainComments.ForEach(x =>
                                {
                                    table.Cell().Row(rowIndex).Column(2).Border(1, Unit.Point).PaddingHorizontal(1).AlignCenter().Text(string.Empty);
                                    table.Cell().Row(rowIndex).Column(3).Border(1, Unit.Point).PaddingLeft(5).Text(x.Comment).LineHeight(1);
                                    rowIndex++;
                                });
                            }

                            foreach (var fileComment in groupedComments)
                            {
                                var comments = fileComment.ToList();
                                table.Cell().Row(rowIndex).Column(1).RowSpan((uint)comments.Count).Border(1, Unit.Point).PaddingLeft(5).Text(fileComment.Key);
                                comments.ForEach(x =>
                                {
                                    table.Cell().Row(rowIndex).Column(2).Border(1, Unit.Point).PaddingHorizontal(1).AlignCenter().Text(x.LineSpanText);
                                    table.Cell().Row(rowIndex).Column(3).Border(1, Unit.Point).PaddingLeft(5).Text(x.Comment).LineHeight(1);
                                    rowIndex++;
                                });
                            }
                        });

                        grid.Item(12).AlignRight().Text($"Total: {dataSource.CodeReviewComments.Count}").Bold().FontSize(12);
                    });

                page.Footer()
                    .Grid(grid =>
                    {
                        grid.Item(4).AlignLeft().Text(x => { x.Span("Page "); x.CurrentPageNumber(); });
                        grid.Item(4);
                        grid.Item(4).AlignRight().Text(x => { x.Span("Created On: "); x.Span(DateTime.Now.ToString()); });
                    });
            });
        }
    }
}
