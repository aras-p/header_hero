using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.Json;

namespace HeaderHero.Utils;

public static class SystemIncludesLocator
{
    public static List<string> FindSystemIncludes()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return FindLinuxIncludes();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return FindMacIncludes();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return FindWindowsIncludes();
        return [];
    }

    static List<string> FindLinuxIncludes()
    {
        string[] common =
        [
            "/usr/include",
            "/usr/local/include",
            "/usr/include/c++",
            "/usr/include/x86_64-linux-gnu",
            "/usr/lib/gcc"
        ];
        return common.Where(Directory.Exists).ToList();
    }

    static List<string> FindMacIncludes()
    {
        string[] common =
        [
            "/usr/include",
            "/Library/Developer/CommandLineTools/usr/include",
            "/Applications/Xcode.app/Contents/Developer/Toolchains/XcodeDefault.xctoolchain/usr/include",
            "/Applications/Xcode.app/Contents/Developer/Platforms/MacOSX.platform/Developer/SDKs/MacOSX.sdk/usr/include",
            "/Applications/Xcode.app/Contents/Developer/Platforms/MacOSX.platform/Developer/SDKs/MacOSX.sdk/usr/include/c++/v1",
        ];
        return common.Where(Directory.Exists).ToList();
    }

    [SupportedOSPlatform("windows")]
    static List<string> FindWindowsIncludes()
    {
        var paths = new List<string>();
        var msvcInclude = GetMsvcIncludePath();
        if (msvcInclude != null)
            paths.Add(msvcInclude);
        var sdkIncludes = GetWindowsSdkIncludePaths();
        paths.AddRange(sdkIncludes);
        return paths;
    }

    // Starting with Visual Studio 2017, "vswhere" utility
    // is used for location detections, see
    // https://devblogs.microsoft.com/cppblog/finding-the-visual-c-compiler-tools-in-visual-studio-2017/
    [SupportedOSPlatform("windows")]
    static string FindVsWhere()
    {
        var progFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        if (string.IsNullOrEmpty(progFiles))
            return null;
        var path = Path.Combine(progFiles, @"Microsoft Visual Studio\Installer\vswhere.exe");
        return File.Exists(path) ? path : null;
    }

    [SupportedOSPlatform("windows")]
    static string GetLatestVsInstallPath()
    {
        var whereTool = FindVsWhere();
        if (whereTool == null)
            return null;

        var psi = new ProcessStartInfo
        {
            FileName = whereTool,
            Arguments = "-latest -format json -prerelease -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64",
            RedirectStandardOutput = true,
            UseShellExecute = false
        };
        using var proc = Process.Start(psi);
        if (proc == null)
            return null;
        var json = proc.StandardOutput.ReadToEnd();
        proc.WaitForExit();
        if (string.IsNullOrWhiteSpace(json))
            return null;

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
            return null;
        var elem = root[0];
        if (elem.TryGetProperty("installationPath", out var pathProp))
            return pathProp.GetString();
        return null;
    }

    [SupportedOSPlatform("windows")]
    static string GetMsvcIncludePath()
    {
        var vs = GetLatestVsInstallPath();
        if (vs == null)
            return null;
        var tools = Path.Combine(vs, @"VC\Tools\MSVC");
        if (!Directory.Exists(tools))
            return null;
        var latest = Directory.GetDirectories(tools).OrderByDescending(Path.GetFileName).FirstOrDefault();
        if (latest == null)
            return null;
        return Path.Combine(latest, "include");
    }

    [SupportedOSPlatform("windows")]
    static List<string> GetWindowsSdkIncludePaths()
    {
        using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
            @"SOFTWARE\Microsoft\Windows Kits\Installed Roots");

        if (key?.GetValue("KitsRoot10") is not string root)
            return [];

        var incRoot = Path.Combine(root, "Include");
        if (!Directory.Exists(incRoot))
            return [];

        var latest = Directory.GetDirectories(incRoot).OrderByDescending(Path.GetFileName).FirstOrDefault();
        if (latest == null)
            return [];

        return
        [
            Path.Combine(latest, "ucrt"),
            Path.Combine(latest, "um"),
            Path.Combine(latest, "shared"),
            Path.Combine(latest, "winrt")
        ];
    }
}