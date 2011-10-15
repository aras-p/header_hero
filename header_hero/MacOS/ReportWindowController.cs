using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.WebKit;
using System.IO;

namespace HeaderHero
{
	public partial class ReportWindowController : MonoMac.AppKit.NSWindowController
	{
		Data.Project _project;
		Parser.Scanner _scanner;
		Parser.Analytics _analytics;
		IncludeData _include_data;
		
		class IncludeData : NSTableViewDataSource
		{
			Parser.Analytics _analytics;
			public List<string> IncludedBy = new List<string>();
			public string File = "";
			public List<string> Includes = new List<string>();
			
			public IncludeData(Parser.Analytics a)
			{
				_analytics = a;
			}
			
			public override int GetRowCount (NSTableView tableView)
			{
				int count = 1;
				if (IncludedBy.Count > count) count = IncludedBy.Count;
				if (Includes.Count > count) count = Includes.Count;
				return count;
			}
			
			public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, int row)
			{
				if (tableColumn == null)
					return (NSString)"";
				
				string id = (NSString)tableColumn.Identifier;
				if (id == @"included_by" && row < IncludedBy.Count) {
					string s = IncludedBy[row];
					string text = string.Format("{0} ({1})", Path.GetFileName(s), _analytics.Items[s].AllIncludedBy.Count);
					return (NSString)text;
				} else if (id == @"file" && row == 0)
					return (NSString)Path.GetFileName (File);
				else if (id == @"includes" && row < Includes.Count) {
					string s = Includes[row];
					string text = string.Format("{0} ({1})", Path.GetFileName(s), _analytics.Items[s].AllIncludes.Count);
					return (NSString)text;
				} else
					return (NSString)"";
			}
		}
		
		class IncludeDelegate : NSTableViewDelegate
		{
			public override NSIndexSet GetSelectionIndexes (NSTableView tableView, NSIndexSet proposedSelectionIndexes)
			{
				return new NSIndexSet();
			}
		}	
		
		#region Constructors
		
		// Called when created from unmanaged code
		public ReportWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ReportWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public ReportWindowController () : base ("ReportWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
			
		}
		
		public void Setup(Data.Project project, Parser.Scanner scanner)
        {
            _project = project;
            _scanner = scanner;
            _analytics = Parser.Analytics.Analyze(_project);
			_include_data = new IncludeData(_analytics);
        }
		
		public override void AwakeFromNib()
		{
			tabView.SelectAt(0);
			
			string errors = String.Join ("\n", _scanner.Errors);
			errorsTextView.TextStorage.SetString(new NSAttributedString(errors));
			
			string missing = String.Join ("\n", _scanner.NotFound.OrderBy(s => s));
			missingFilesTextView.TextStorage.SetString(new NSAttributedString(missing));
			
            string file = Parser.Report.Generate(_project, _analytics);
			
            if (reportView.MainFrameUrl != null)
                reportView.Reload(this);
            else {
				NSUrl url = NSUrl.FromFilename(file);
                reportView.MainFrameUrl = url.ToString();
			}
			
			reportView.DecidePolicyForNavigation += (sender, e) => {
				NSUrl url = e.Request.Url;
				if (url.Host == "inspect")
	            {
	                string inspect = url.Query;
	                Inspect(Uri.UnescapeDataString(inspect));
	                tabView.SelectAt(1);
	            }
			};
			
			includesTableView.DataSource = _include_data;
			includesTableView.Delegate = new IncludeDelegate();
			includesTableView.DoubleClick += (sender, e) => {
				int col = includesTableView.ClickedColumn;
				int row = includesTableView.ClickedRow;
				if (col == 0 && row < _include_data.IncludedBy.Count)
					Inspect (_include_data.IncludedBy[row]);
				else if (col == 1 && row == 0)
					Inspect (_include_data.File);
				else if (col == 2 && row < _include_data.Includes.Count)
					Inspect (_include_data.Includes[row]);
			};
		}
		
		#endregion
		
		private void Inspect(string file)
        {
			_include_data.IncludedBy = 
				_project.Files.Where(kvp => kvp.Value.AbsoluteIncludes.Contains(file)).Select(kvp => kvp.Key)
                	.OrderByDescending(s => _analytics.Items[s].AllIncludedBy.Count)
					.ToList();
					
			_include_data.File = file;
			
			_include_data.Includes =
				_project.Files[file].AbsoluteIncludes.OrderByDescending(f => _analytics.Items[f].AllIncludes.Count)
					.ToList();
			
			includesTableView.ReloadData();
        }
		
		//strongly typed window accessor
		public new ReportWindow Window {
			get {
				return (ReportWindow)base.Window;
			}
		}
	}
}

