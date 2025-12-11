using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using HeaderHero.Serialization;

namespace HeaderHero;

public partial class MainWindow : Window
{
    static readonly FilePickerFileType[] PickerFileTypes =
    [
        new("Header Hero") { Patterns = ["*.header_hero"] }
    ];

    string _curProjectPath;
    string _lastSaveProjectState;
    Data.Project _project = new();

    public MainWindow()
    {
        InitializeComponent();
        if (OperatingSystem.IsMacOS())
            MenuBar.IsVisible = false;

        Closing += OnWindowClosing;

        //@TODO
        //projectDirsTextBox.MouseDoubleClick += (_1, _2) => scan_AddDirectory_Click(_1, null);
        //includeDirsTextBox.MouseDoubleClick += (_1, _2) => include_AddDirectory_Click(_1, null);

        _lastSaveProjectState = Sjson.Encode(_project.ToDict());

        var settings = AppSettings.Instance;
        var lastProject = settings.LastProject;
        if (!string.IsNullOrEmpty(lastProject) && File.Exists(lastProject))
        {
            Open(lastProject);
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
        _project.PrecompiledHeader = PchTextBox.Text?.Trim() ?? "";
    }

    void MarkSave()
    {
        if (_curProjectPath != null)
            Title = "Header Hero - " + _curProjectPath;
        else
            Title = "Header Hero";
        _lastSaveProjectState = Sjson.Encode(_project.ToDict());
    }

    async Task<bool> AskSaveProject()
    {
        ParseProject();
        if (_lastSaveProjectState == Sjson.Encode(_project.ToDict()))
            return true;

        int choice = await new MessageBox3("Save Project?", "Project was modified, save changes?", "Save", "Do not save", "Cancel").ShowDialog<int>(this);
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

        AppSettings.Instance.LastProject = string.Empty;
        AppSettings.Instance.Save();
        _curProjectPath = null;
        _project = new Data.Project();
        DisplayProject();
        MarkSave();
    }

    async void OpenProject()
    {
        if (!await AskSaveProject())
            return;

        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions {AllowMultiple = false, FileTypeFilter = PickerFileTypes});
        if (files is not {Count: > 0})
            return;

        var path = files[0].TryGetLocalPath();
        AppSettings.Instance.LastProject = path;
        AppSettings.Instance.Save();
        Open(path);
    }

    void Open(string path)
    {
        _curProjectPath = path;
        _project = new Data.Project();
        _project.FromDict(Sjson.Load(path));
        MarkSave();
        DisplayProject();
    }

    async void SaveProject(bool force_save_as)
    {
        if (_curProjectPath == null || force_save_as)
        {
            var result = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions {SuggestedFileName = _curProjectPath != null ? Path.GetFileName(_curProjectPath) : "project.header_hero", FileTypeChoices = PickerFileTypes});
            if (result == null)
                return;
            _curProjectPath = result.TryGetLocalPath();
        }

        if (_curProjectPath == null)
            return;

        AppSettings.Instance.LastProject = _curProjectPath;
        AppSettings.Instance.Save();
        ParseProject();
        Sjson.Save(_project.ToDict(), _curProjectPath);
        MarkSave();
    }

    async void OnWindowClosing(object sender, WindowClosingEventArgs e)
    {
        e.Cancel = true;
        bool allow = await AskSaveProject();
        if (allow)
        {
            Closing -= OnWindowClosing;
            Close();
        }
    }

    async void ScanProject()
    {
        ParseProject();
        var scanner = new Parser.Scanner(_project);

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
