// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace HeaderHero
{
	[Register ("ReportWindow")]
	partial class ReportWindow
	{
	}

	[Register ("ReportWindowController")]
	partial class ReportWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSTabView tabView { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView includesTableView { get; set; }

		[Outlet]
		MonoMac.WebKit.WebView reportView { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextView errorsTextView { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextView missingFilesTextView { get; set; }
	}
}
