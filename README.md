# TFSCodeReviewTool

A console application to produce a report for a TFS Code Review.

## Commands

| Command(s) | Required? | Default Value | Description |
| --- | --- | --- | --- |
| -p, --ProjectName | ✔️ | | The name of the project in Azure Devops that contains the review(s) |
| -w, --ReviewWorkItemIds | ✔️ | | A list of work item ids that you would like to print a report for, separated by a space |
| --ir | ❌ | `false` | If present then an individual report will be created for each ReviewWorkItemId provided, otherwise a single "merged" report will be created |
| --slfp | ❌ | Current user's Documents folder | Custom file path to save the reports to |
| --fnfs | ❌ | `"{0}-{1}-{2}"` | A format string for the filename <ul><li>{0} = Project Name</li><li>{1} = Work Item Number (This is 'Merged' for merged review)</li><li>{2} = Current Date Time</li></ul> |
| --dtfs | ❌ | [`"d"`](https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings#dSpecifier) | The format that the date will be in for the filename |
