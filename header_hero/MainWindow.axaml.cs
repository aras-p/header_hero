using System.Linq;
using Avalonia.Controls;
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
        QuitMenu.Click += (_, _) => this.Close();
        this.Closing += OnWindowClosing;

        //@TODO
        //projectDirsTextBox.MouseDoubleClick += (_1, _2) => scan_AddDirectory_Click(_1, null);
        //includeDirsTextBox.MouseDoubleClick += (_1, _2) => include_AddDirectory_Click(_1, null);

        //@TODO
        //string last = Properties.Settings.Default.LastProject;
        //if (last != null && last != "" && File.Exists(last))
        //{
        //    Open(last);
        //    scanToolStripMenuItem1_Click(null, null);
        //}
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
        Open(path);
    }

    void Open(string path)
    {
        _file = path;
        //Properties.Settings.Default.LastProject = _file; //@TODO
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

        //Properties.Settings.Default.LastProject = _file; //@TODO
        ParseProject();
        Sjson.Save(JsonSerializer.Save(_project), _file);
        MarkSave();
    }

    void OnWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        // Properties.Settings.Default.Save(); //@TODO
        if (!CheckSave())
            e.Cancel = true;
    }
}
