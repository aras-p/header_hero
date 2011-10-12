// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace HeaderHero
{
	[Register ("MainWindow")]
	partial class MainWindow
	{
	}

	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		public MonoMac.AppKit.NSTextView projectDirsTextView { get; set; }

		[Outlet]
		public MonoMac.AppKit.NSTextView includeDirsTextView { get; set; }

		[Outlet]
		public MonoMac.AppKit.NSTextField lastScanTextField { get; set; }
	}
}
