using CommandLine;
using System.Collections.Generic;

namespace TFSCodeReviewTool
{
    public class Options
    {
        [Option('p', Required = true, HelpText = "The name of the project that the contain the review(s)")]
        public string ProjectName { get; set; }

        [Option('w', Required = true, HelpText = "List of work item ids separated by spaces", Separator = ' ')]
        public IEnumerable<int> ReviewWorkItemIds { get; set; }

        [Option("ir", Required = false, HelpText = "Creates individual reports for each code review", Default = false)]
        public bool IndividualReports { get; set; }

        [Option("slfp",
            Required = false,
            HelpText = "File Path to save the reports to.\nDefaults to current user's Documents folder")]
        public string SaveLocationFilePath { get; set; }

        [Option("fnfs",
            Required = false,
            HelpText = "A format string for the filename\n{0} = Project Name\n{1} = Work Item Number (This is 'Merged' for merged review\n{2} = Current Date Time",
            Default = "{0}-{1}-{2}")]
        public string FileNameFormatString { get; set; }

        [Option("dtfs",
            Required = false,
            HelpText = "A format string for the datetime used in the filename",
            Default = "d")]
        public string DateTimeFormatString { get; set; }
    }
}
