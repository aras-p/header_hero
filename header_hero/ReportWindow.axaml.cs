using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using HeaderHero.Parser;

namespace HeaderHero;

public record IncludeRow(string Name, string FullPath, int Count, int Lines);
public record MissingFilesRow(string Name, string Origin);

public partial class ReportWindow : Window
{
    Data.Project _project;
    Analytics _analytics;
    Scanner _scanner;
    readonly LinkedList<string> _history = [];

    public ReportWindow(Data.Project project, Scanner scanner)
    {
        _history.Clear();
        _project = project;
        _scanner = scanner;
        InitializeComponent();

        GetTopLevel(this)!.Cursor = new Cursor(StandardCursorType.Wait);
        Setup(project, scanner);
        GetTopLevel(this)!.Cursor = new Cursor(StandardCursorType.Arrow);
    }

    void ReportFileList_OnDoubleTapped(object sender, TappedEventArgs e)
    {
        if (sender is not ListBox lb) return;
        if (lb.SelectedItem is not ReportFile rf) return;
        Inspect(rf.path);
        Tabs.SelectedIndex = 1;
    }

    void IncludesList_OnDoubleTapped(object sender, TappedEventArgs e)
    {
        if (sender is not DataGrid dg) return;
        if (dg.SelectedItem is not IncludeRow row) return;
        Inspect(row.FullPath);
    }

    void BackButton_OnClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_history.Count > 0)
            _history.RemoveLast();
        if (_history.Count > 0)
            Inspect(_history.Last!.Value);
    }

    void Inspect(string file)
    {
        if (_history.Count == 0 || _history.Last() != file)
        {
            _history.AddLast(file);
            if (_history.Count > 10)
                _history.RemoveFirst();
        }

        // center text
        {
            var projectFile = _project.Files[file];
            var analyticsFile = _analytics.Items[file];
            var fileLines = projectFile.Lines;
            var directLines = projectFile.AbsoluteIncludes.Sum(f => _project.Files[f].Lines);
            var directCount = projectFile.AbsoluteIncludes.Count;
            var totalLines = analyticsFile.TotalIncludeLines;
            var totalCount = analyticsFile.AllIncludes.Count;
            string text = $"{Path.GetFileName(file)}\r\n\r\nLines: {fileLines}\r\nDirect Includes: {directLines} lines, {directCount} files\r\nTotal Includes: {totalLines} lines, {totalCount} files";
            CurrentFileLabel.Text = Path.GetFileName(file);
            FileDetailsText.Text = text;
        }

        // left panel
        {
            List<IncludeRow> list = _project.Files[file].AbsoluteIncludes
                .OrderByDescending(f => _analytics.Items[f].AllIncludes.Count)
                .Select(s => new IncludeRow(Path.GetFileName(s), s, _analytics.Items[s].AllIncludes.Count, _analytics.Items[s].TotalIncludeLines)).ToList();
            IncludesList.ItemsSource = list;
        }

        // right panel
        {
            IEnumerable<string> included = _project.Files.Where(kvp => kvp.Value.AbsoluteIncludes.Contains(file)).Select(kvp => kvp.Key);
            List<IncludeRow> list = included.OrderByDescending(s => _analytics.Items[s].AllIncludedBy.Count)
                .Select(s => new IncludeRow(Path.GetFileName(s), s, _analytics.Items[s].AllIncludedBy.Count, _analytics.Items[s].TotalIncludeLines)).ToList();
            IncludedByList.ItemsSource = list;
        }
    }

    void Setup(Data.Project project, Scanner scanner)
    {
        _history.Clear();
        _project = project;
        _scanner = scanner;
        _analytics = Analytics.Analyze(_project);

        ErrorsList.ItemsSource = scanner.Errors;
        var notFoundItems = scanner.NotFound
            .OrderBy(s => s)
            .Select(s => new MissingFilesRow(s, _scanner.NotFoundOrigins[s]))
            .ToList();
        MissingFilesList.ItemsSource = notFoundItems;

        Report rpt = new Report(_project, _analytics);
        SummaryText.Text = rpt.summary;

        BiggestList.ItemsSource = rpt.largest;
        HeaderHubsList.ItemsSource = rpt.hubs;
        PrecompiledHeadersList.ItemsSource = rpt.precompiled;
    }
}