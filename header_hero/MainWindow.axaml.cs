using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Threading;
using HeaderHero.Serialization;

namespace HeaderHero;

public partial class MainWindow : Window
{
    string _file;
    string _last_save;
    HeaderHero.Data.Project _project = new();

    public MainWindow()
    {
        InitializeComponent();
        //this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); //@TODO

        NewProjectMenu.Click += (_, _) => NewProject();
        OpenProjectMenu.Click += (_, _) => OpenProject();
        SaveProjectMenu.Click += (_, _) => SaveProject();
        CloseProjectMenu.Click += (_, _) => NewProject();
        ScanMenu.Click += (_, _) => ScanProject();
        CleanRescanMenu.Click += (_, _) => ClearRescanProject();
        QuitMenu.Click += (_, _) => this.Close();
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
        LastScanLabel.Text = _project.LastScan.ToString();
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
        _last_save = Sjson.Encode(JsonSerializer.Save(_project));
    }

    bool CheckSave()
    {
        ParseProject();
        if (_last_save == Sjson.Encode(JsonSerializer.Save(_project)))
            return true;
        SaveProject();
        return true;
    }

    void NewProject()
    {
        if (!CheckSave())
            return;

        _file = null;
        _project = new HeaderHero.Data.Project();
        DisplayProject();
        MarkSave();
    }

    async void OpenProject()
    {
        if (!CheckSave())
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
        _project = new HeaderHero.Data.Project();
        JsonSerializer.Load(_project, Sjson.Load(path));
        MarkSave();
        DisplayProject();
    }

    async void SaveProject()
    {
        if (_file == null)
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
        Sjson.Save(JsonSerializer.Save(_project), _file);
        MarkSave();
    }

    void OnWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        if (!CheckSave())
            e.Cancel = true;
    }

    async void ScanProject()
    {
        ParseProject();
        var scanner = new Parser.Scanner(_project);

        DateTime started = DateTime.Now;

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

        _project.LastScan = started;
        DisplayProject();

        ReportWindow report = new ReportWindow(_project, scanner);
        report.Show();
    }

    void ClearRescanProject()
    {
        _project.Clean();
        ScanProject();
    }
}
