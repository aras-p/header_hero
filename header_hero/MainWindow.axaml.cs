using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using HeaderHero.Utils;

namespace HeaderHero;

public partial class MainWindow : Window
{
    static readonly FilePickerFileType[] PickerFileTypes =
    [
        new("JSON") { Patterns = ["*.json"] }
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

        _lastSaveProjectState = _project.ToJson();

        var settings = AppSettings.Instance;
        var lastProject = settings.LastProject;
        if (!string.IsNullOrEmpty(lastProject) && File.Exists(lastProject))
        {
            Open(lastProject);
        }
    }

    void ProjectFieldsToUI()
    {
        ProjectDirsTextBox.Text = string.Join("\r\n", _project.ScanDirectories.ToArray());
        IncludeDirsTextBox.Text = string.Join("\r\n", _project.IncludeDirectories.ToArray());
        PchTextBox.Text = _project.PrecompiledHeader ?? string.Empty;
    }

    void ParseProjectFieldsFromUI()
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
        _lastSaveProjectState = _project.ToJson();
    }

    async Task<bool> AskSaveProject()
    {
        ParseProjectFieldsFromUI();
        if (_lastSaveProjectState == _project.ToJson())
            return true;

        int choice = await new MessageBox3("Save Project?", "Project was modified, save changes?", "Save", "Do not save", "Cancel").ShowDialog<int>(this);
        switch (choice)
        {
            case 0: return await SaveProject(false);
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
        ProjectFieldsToUI();
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
        _project.FromJsonFile(path);
        MarkSave();
        ProjectFieldsToUI();
    }

    async Task<bool> SaveProject(bool force_save_as)
    {
        if (_curProjectPath == null || force_save_as)
        {
            var result = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions {SuggestedFileName = _curProjectPath != null ? Path.GetFileName(_curProjectPath) : "header_hero_project.json", FileTypeChoices = PickerFileTypes});
            if (result == null)
                return false;
            _curProjectPath = result.TryGetLocalPath();
        }

        if (_curProjectPath == null)
            return false;

        AppSettings.Instance.LastProject = _curProjectPath;
        AppSettings.Instance.Save();
        ParseProjectFieldsFromUI();
        File.WriteAllText(_curProjectPath, _project.ToJson());
        MarkSave();
        return true;
    }

    bool _isCloseRequested;

    void OnWindowClosing(object sender, WindowClosingEventArgs e)
    {
        if (_isCloseRequested)
            return;
        e.Cancel = true;

        // Since project save ask will show async dialogs, we have to do that in regular
        // UI loop.
        Dispatcher.UIThread.Post(async () =>
        {
            bool allow = await AskSaveProject();
            if (allow)
            {
                _isCloseRequested = true;
                Close();
            }
        });
    }

    async void ScanProject()
    {
        ParseProjectFieldsFromUI();
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

        ProjectFieldsToUI();

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

    async void Menu_SaveProject(object sender, EventArgs e)
    {
        await SaveProject(false);
    }

    async void Menu_SaveProjectAs(object sender, EventArgs e)
    {
        await SaveProject(true);
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
