using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using System.Linq;
using HeaderHero.Serialization;

namespace HeaderHero
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;
		string _file;
        string _last_save;
        Data.Project _project;
		
		public AppDelegate ()
		{
		}

		public override void FinishedLaunching (NSObject notification)
		{
			mainWindowController = new MainWindowController ();
			mainWindowController.Window.MakeKeyAndOrderFront (this);
			
			string last_project = Properties.Settings.Default.LastProject;
			_project = new Data.Project();
            if (last_project != null && last_project != "" && System.IO.File.Exists(last_project)) {
				Open (last_project);
			}
			DisplayProject();
		}
		
		public override NSApplicationTerminateReply ApplicationShouldTerminate(NSApplication sender)
		{
			if (CheckSave ())
				return NSApplicationTerminateReply.Now;
			else
				return NSApplicationTerminateReply.Cancel;
		}
		
		#region Project
		
		private void DisplayProject()
        {
			mainWindowController.lastScanTextField.StringValue = _project.LastScan.ToString();
			
			NSAttributedString projects = new NSAttributedString(String.Join("\n", _project.ScanDirectories.ToArray()));
            mainWindowController.projectDirsTextView.TextStorage.SetString ( projects );
			
			NSAttributedString includes = new NSAttributedString(String.Join("\n", _project.IncludeDirectories.ToArray()));
            mainWindowController.includeDirsTextView.TextStorage.SetString ( includes);
        }

        private void ParseProject()
        {
			string projects = mainWindowController.projectDirsTextView.TextStorage.Value;
            _project.ScanDirectories = projects.Trim().Split('\n').Where(s => s.Trim().Length > 0).ToList();
            
			string includes = mainWindowController.projectDirsTextView.TextStorage.Value;
			_project.IncludeDirectories = includes.Trim().Split('\n').Where(s => s.Trim().Length > 0).ToList();
        }
		
		#endregion
		
		#region File Menu
		
		private bool CheckSave()
        {
            ParseProject();
            if (_last_save == Sjson.Encode(JsonSerializer.Save(_project)))
                return true;
			
			NSAlert alert = new NSAlert();
			alert.AddButton(@"Save");
			alert.AddButton(@"Don't Save");
			alert.AddButton(@"Cancel");
			alert.MessageText = @"Do you want to save changes to this document before closing?";
			alert.InformativeText = @"If you don't save, your changes will be lost.";
			alert.AlertStyle = NSAlertStyle.Informational;
			
			int result = alert.RunModal();
			if (result == (int)NSAlertButtonReturn.First) {
				saveProject (null);
				return _file != null;
			} else if (result == (int)NSAlertButtonReturn.Second)
				return true;
			else
				return false;
        }
		
		private void MarkSave()
        {
            if (_file != null)
                mainWindowController.Window.Title = "Header Hero - " + _file;
            else
                mainWindowController.Window.Title = "Header Hero";
            _last_save = Sjson.Encode( JsonSerializer.Save(_project) );
        }
		
		partial void newProject (NSObject sender)
		{
			if (CheckSave()) {
	            _file = null;
	            _project = new Data.Project();
	            DisplayProject();
	            MarkSave();
			}
		}
		
		partial void closeProject (NSObject sender)
		{
			NSWindow window = NSApplication.SharedApplication.MainWindow;
			if (window == mainWindowController.Window)
				newProject (sender);
			else
				window.PerformClose(sender);
		}
		
		private void Open(string path)
        {
            _file = path;
            Properties.Settings.Default.LastProject = _file;
			Properties.Settings.Default.Save();
            _project = new Data.Project();
            JsonSerializer.Load(_project, Sjson.Load(_file));
            MarkSave();
            DisplayProject();
        }
		
		void Save(string path)
		{
			_file = path;
   			Properties.Settings.Default.LastProject = _file;
			Properties.Settings.Default.Save();
            ParseProject();
            Sjson.Save(JsonSerializer.Save(_project), _file);
            MarkSave();
		}
		
		partial void openProject (NSObject sender)
		{
			if (CheckSave()) {
				NSOpenPanel panel = NSOpenPanel.OpenPanel;
				panel.AllowedFileTypes = new string[] {@"header_hero"};
				panel.BeginSheet (mainWindowController.Window, (result) => {
					if (result == (int)NSPanelButtonType.Ok)
						Open(panel.Url.Path);
				});
			}
		}
		
		partial void saveProject (NSObject sender)
		{
			if (_file == null)
            {
				saveProjectAs (sender);
               	return;
            }
			Save (_file);
		}
		
		partial void saveProjectAs (NSObject sender)
		{
			NSSavePanel panel = NSSavePanel.SavePanel;
			panel.AllowedFileTypes = new string[] {@"header_hero"};
			panel.BeginSheet (mainWindowController.Window, (result) => {
				if (result == (int)NSPanelButtonType.Ok)
					Save(panel.Url.Path);
			});
		}
		
		#endregion
		
		#region Scan Menu
		
		partial void scan (NSObject sender)
		{
			ParseProject();
            Parser.Scanner scanner = new Parser.Scanner(_project);
            
            DateTime started = DateTime.Now;
			ProgressDialog d = new ProgressDialog();
            d.Text = "Scanning source files...";
            d.Work = (feedback) => scanner.Rescan(feedback);
            d.Start();
            _project.LastScan = started;

            DisplayProject();
			
			ReportWindowController rwc = new ReportWindowController();
			rwc.Setup (_project, scanner);
			rwc.Window.MakeKeyAndOrderFront(this);
		}
		
		partial void cleanRescan (NSObject sender)
		{
			 _project.Clean();
            scan (sender);
		}
		
		#endregion
	}
}

