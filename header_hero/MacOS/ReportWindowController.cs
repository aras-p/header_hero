using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.WebKit;

namespace HeaderHero
{
	public partial class ReportWindowController : MonoMac.AppKit.NSWindowController
	{
		Data.Project _project;
		Parser.Scanner _scanner;
		Parser.Analytics _analytics;
		
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
		}
		
		#endregion
		
		private void Inspect(string file)
        {
            /*
            {
                fileListView.Items.Clear();
                ListViewItem item = new ListViewItem(Path.GetFileName(file));
                item.Tag = file;
                fileListView.Items.Add(item);
            }

            {
                includesListView.Items.Clear();
                foreach (string s in _project.Files[file].AbsoluteIncludes.OrderByDescending(f => _analytics.Items[f].AllIncludes.Count))
                {
                    string text = string.Format("{0} ({1})", Path.GetFileName(s), _analytics.Items[s].AllIncludes.Count);
                    ListViewItem item = new ListViewItem(text);
                    item.Tag = s;
                    includesListView.Items.Add(item);
                }
            }

            {
                includedByListView.Items.Clear();
                IEnumerable<string> included = _project.Files.Where(kvp => kvp.Value.AbsoluteIncludes.Contains(file)).Select(kvp => kvp.Key);
                foreach (string s in included.OrderByDescending(s => _analytics.Items[s].AllIncludedBy.Count))
                {
                    string text = string.Format("{0} ({1})", Path.GetFileName(s), _analytics.Items[s].AllIncludedBy.Count);
                    ListViewItem item = new ListViewItem(text);
                    item.Tag = s;
                    includedByListView.Items.Add(item);
                }
            }
            */
        }
		
		//strongly typed window accessor
		public new ReportWindow Window {
			get {
				return (ReportWindow)base.Window;
			}
		}
	}
}

