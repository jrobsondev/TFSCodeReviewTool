﻿using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Linq;
using TFSCodeReviewTool.Properties;
using TFSCodeReviewTool.ReportDataSources;
using TFSCodeReviewTool.ReportElements;
using TFSCodeReviewTool.Reports;

namespace TFSCodeReviewTool
{
    internal class ReportManager
    {
        private string _SaveLocation;
        private CodeReviewReport _CodeReviewReport;

        public ReportManager(string saveLocation)
        {
            _SaveLocation = saveLocation;
            _CodeReviewReport = new CodeReviewReport();
        }

        public void GenerateCodeReviewReportPDF(CodeReviewReportDataSource dataSource)
        {
            var codeReview = dataSource.CodeReview;
            var groupedComments = dataSource.CodeReviewComments.Where(x => !x.IsMainComment).GroupBy(x => x.FileName);
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .AlignCenter()
                        .Text($"{codeReview.ProjectName} - Code Review: {codeReview.ReviewNumber}")
                        .SemiBold().FontSize(25).Underline();

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Grid(grid =>
                        {
                            grid.VerticalSpacing(1, Unit.Centimetre);
                            grid.Item(6).Grid(row =>
                            {
                                row.Item(4).Text("Associated Changeset:").Bold(); row.Item(6).Text(codeReview.ReviewForChangeset);
                                row.Item(4).Text("Review Created By:").Bold(); row.Item(6).Text(codeReview.ReviewForName);
                                row.Item(4).Text("Review Created On:").Bold(); row.Item(6).Text(codeReview.ReviewSentOnDateTime);
                                row.Item(4).Text("Reviewed By").Bold(); row.Item(6).Text(codeReview.ReviewByName);
                                row.Item(4).Text("Reviewed On:").Bold(); row.Item(6).Text(codeReview.ReviewCompletedOnDateTime);
                            });

                            grid.Item(6).Border(1, Unit.Point).Grid(row =>
                            {
                                row.Item(12).Background(Colors.Grey.Lighten3).Border(1, Unit.Point).PaddingLeft(5).Text("Overall Comments").Bold();
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
                                foreach (var fileComment in groupedComments)
                                {
                                    var comments = fileComment.ToList();
                                    table.Cell().Row(rowIndex).Column(1).RowSpan((uint)comments.Count).Border(1, Unit.Point).PaddingLeft(5).Text(fileComment.Key);
                                    foreach (var comment in comments)
                                    {
                                        table.Cell().Row(rowIndex).Column(2).Border(1, Unit.Point).PaddingHorizontal(1).AlignCenter().Text(comment.LineSpanText);
                                        table.Cell().Row(rowIndex).Column(3).Border(1, Unit.Point).PaddingLeft(5).Text(comment.Comment).LineHeight(1);
                                        rowIndex++;
                                    }
                                }
                            });
                        });

                    page.Footer()
                        .Grid(grid =>
                        {
                            grid.Item(4).AlignLeft().Text(x => { x.Span("Page "); x.CurrentPageNumber(); });
                            grid.Item(4);
                            grid.Item(4).AlignRight().Text(x => { x.Span("Created On: "); x.Span(DateTime.Now.ToString()); });
                        });
                });
            }).GeneratePdf(_SaveLocation);
        }
    }
}