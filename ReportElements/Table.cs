using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace TFSCodeReviewTool.ReportElements
{
    internal static class Table
    {
        internal static IContainer Header(IContainer container)
        {
            return container
                .Border(1)
                .Background(Colors.Grey.Lighten3)
                .ShowOnce()
                .AlignCenter()
                .AlignMiddle();
        }
    }
}
