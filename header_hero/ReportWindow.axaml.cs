using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using HeaderHero.Parser;

namespace HeaderHero;

public partial class ReportWindow : Window
{
    Data.Project _project;
    Analytics _analytics;
    Parser.Scanner _scanner;

    public ReportWindow(Data.Project project, Parser.Scanner scanner)
    {
        //_history.Clear(); //@TODO
        _project = project;
        _scanner = scanner;
        InitializeComponent();

        //@TODO
        //includedByListView.MouseDoubleClick += new MouseEventHandler(includedByListView_MouseDoubleClick);
        //includesListView.MouseDoubleClick += new MouseEventHandler(includedByListView_MouseDoubleClick);

        //Cursor.Current = Cursors.WaitCursor; //@TODO
        Setup(project, scanner);
        //Cursor.Current = Cursors.Default; //@TODO
    }

    void OnBiggestDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (BiggestList.SelectedItem is ReportFile rf)
        {
            Inspect(rf.path);
            Tabs.SelectedIndex = 1; // we'll build the Includes tab next
        }
    }

    private void Inspect(string file)
    {
        // next step will fill this out
        Console.WriteLine("INSPECT: " + file);
    }

    private void Setup(Data.Project project, Parser.Scanner scanner)
    {
        //_history.Clear(); //@TODO
        _project = project;
        _scanner = scanner;
        _analytics = Parser.Analytics.Analyze(_project);

        /*@TODO
        errorsListView.Items.Clear();
        foreach (string s in scanner.Errors)
            errorsListView.Items.Add(s);
        missingFilesListView.Items.Clear();
        foreach (var s in scanner.NotFound.OrderBy(s => s))
        {
            var li = new ListViewItem(s);
            li.ToolTipText = scanner.NotFoundOrigins[s];
            missingFilesListView.Items.Add(li);
        }
        */

        Report rpt = new Report(_project, _analytics);
        SummaryText.Text = rpt.summary;

        BiggestList.ItemsSource = rpt.largest;
    }

}