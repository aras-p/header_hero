using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using HeaderHero.Serialization;

namespace HeaderHero;

public partial class MainWindow : Window
{
    string _file;
    string _last_save;
    Data.Project _project = new();

    public MainWindow()
    {
        InitializeComponent();
        if (OperatingSystem.IsMacOS())
            MenuBar.IsVisible = false;

        this.Closing += OnWindowClosing;

        //@TODO
        //projectDirsTextBox.MouseDoubleClick += (_1, _2) => scan_AddDirectory_Click(_1, null);
        //includeDirsTextBox.MouseDoubleClick += (_1, _2) => include_AddDirectory_Click(_1, null);

        var settings = AppSettings.Instance;
        var lastProject = settings.LastProject;
        if (!string.IsNullOrEmpty(lastProject) && File.Exists(lastProject))
        {
            Open(lastProject);
            //ScanProject(); //@TODO
        }
    }

    void DisplayProject()
    {
        ProjectDirsTextBox.Text = string.Join("\r\n", _project.ScanDirectories.ToArray());
        IncludeDirsTextBox.Text = string.Join("\r\n", _project.IncludeDirectories.ToArray());
        PchTextBox.Text = _project.PrecompiledHeader ?? string.Empty;
    }

    void ParseProject()
    {
        _project.ScanDirectories = ProjectDirsTextBox.Text?.Split('\n', '\r').Where(s => !string.IsNullOrWhiteSpace(s)).ToList() ?? [];
        _project.IncludeDirectories = IncludeDirsTextBox.Text?.Split('\n', '\r').Where(s => !string.IsNullOrWhiteSpace(s)).ToList() ?? [];
        _project.PrecompiledHeader = PchTextBox.Text?.Trim();
    }

    void MarkSave()
    {
        if (_file != null)
            this.Title = "Header Hero - " + _file;
        else
            this.Title = "Header Hero";
        _last_save = Sjson.Encode(_project.ToDict());
    }

    async Task<bool> AskSaveProject()
    {
        ParseProject();
        if (_last_save == Sjson.Encode(_project.ToDict()))
            return true;

        int choice = await new MessageBox3("Save Project?", "Project was modified, save changes?", new[]{"Save", "Do not save", "Cancel"}).ShowDialog<int>(this);
        switch (choice)
        {
            case 0: SaveProject(false); break;
            case 1: break;
            case 2: return false;
        }
        return true;
    }

    async void NewProject()
    {
        if (!await AskSaveProject())
            return;

        _file = null;
        _project = new Data.Project();
        DisplayProject();
        MarkSave();
    }

    async void OpenProject()
    {
        if (!await AskSaveProject())
            return;

        var dlg = new OpenFileDialog
        {
            AllowMultiple = false,
            Filters =
            {
                new FileDialogFilter
                {
                    Name = "Header Hero",
                    Extensions = { "header_hero" }
                }
            }
        };

        var result = await dlg.ShowAsync(this);
        if (result == null || result.Length == 0)
            return;

        var path = result[0];
        AppSettings.Instance.LastProject = path;
        AppSettings.Instance.Save();
        Open(path);
    }

    void Open(string path)
    {
        _file = path;
        _project = new Data.Project();
        var sjsondata = Sjson.Load(path);
        _project.FromDict(Sjson.Load(path));
        MarkSave();
        DisplayProject();
    }

    async void SaveProject(bool force_save_as)
    {
        if (_file == null || force_save_as)
        {
            var dlg = new SaveFileDialog
            {
                Filters = { new FileDialogFilter { Name = "Header Hero", Extensions = { "header_hero" } } }
            };
            var chosen = await dlg.ShowAsync(this);
            if (chosen == null)
                return;
            _file = chosen;
        }

        if (_file == null)
            return;

        AppSettings.Instance.LastProject = _file;
        AppSettings.Instance.Save();
        ParseProject();
        Sjson.Save(_project.ToDict(), _file);
        MarkSave();
    }

    async void OnWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        e.Cancel = true;
        bool allow = await AskSaveProject();
        if (allow)
        {
            this.Closing -= OnWindowClosing;
            this.Close();
        }
    }

    async void ScanProject()
    {
        _project.Clean();
        ParseProject();
        var scanner = new Parser.Scanner(_project);

        //DateTime started = DateTime.Now;

        ProgressDialog dlg = new ProgressDialog();
        dlg.Start(async (fb) =>
        {
            scanner.Rescan(fb);
        });

        // Poll every 100ms until the dialog closes
        var timer = new System.Timers.Timer(100);
        timer.Elapsed += (_, _) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (!dlg.IsVisible)
                    timer.Stop();
                else
                    dlg.Poll();
            });
        };
        timer.Start();

        await dlg.ShowDialog(this);

        DisplayProject();

        ReportWindow report = new ReportWindow(_project, scanner);
        report.Show();
    }

    void Menu_NewProject(object sender, EventArgs e)
    {
        NewProject();
    }

    void Menu_OpenProject(object sender, EventArgs e)
    {
        OpenProject();
    }

    void Menu_SaveProject(object sender, EventArgs e)
    {
        SaveProject(false);
    }
    void Menu_SaveProjectAs(object sender, EventArgs e)
    {
        SaveProject(true);
    }

    void Menu_Quit(object sender, EventArgs e)
    {
        Close();
    }

    void Button_Scan(object sender, RoutedEventArgs e)
    {
        ScanProject();
    }
}
