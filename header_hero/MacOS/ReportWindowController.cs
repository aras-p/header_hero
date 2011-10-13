using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

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
				
            // reportBrowser.Navigating += reportBrowser_Navigating;
		}
		
		#endregion
		
		//strongly typed window accessor
		public new ReportWindow Window {
			get {
				return (ReportWindow)base.Window;
			}
		}
	}
}

