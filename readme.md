# Header Hero (Fork)

BitSquid back in 2011 released a nifty tool for analysing C++ header includes, see
[Caring by Sharing: Header Hero](https://bitsquid.blogspot.com/2011/10/caring-by-sharing-header-hero.html) blog post
([webarchive link](https://web.archive.org/web/20250430235425/https://bitsquid.blogspot.com/2011/10/caring-by-sharing-header-hero.html)).
The original Bitbucket repository of that tool is long gone by now.

Back in 2018 I made some functionality and UX improvements to it, see
[Header Hero Improvements](https://aras-p.info/blog/2018/01/17/Header-Hero-Improvements/) blog post.

In 2025 I rewrote the tool from C# WinForms to [Avalonia UI](https://avaloniaui.net/) framework, using a more modern C#/.NET
version as well. So this now works on Windows, Mac and Linux. I did some more UI improvements and performance optimizations too;
header scanning on a large codebase is several times faster than before now.

## Usage

Specify where your project is at, as well as where are the source files under (these folders are scanned recursively),
and where are the include folders. Each line there can be an absolute path, or a path relative to project root.

You can specify a precompiled header (absolute path), if you use one. The tool tries to detect "system" (compiler / platform SDK)
includes automatically and displays them at the bottom.

![Screenshot](/img/hh_main.png?raw=true "Screenshot")

Press the "Scan" button! This will show up the main report window. See the above mentioned blog post for details on what it contains.

| Main | Includes | Errors | Missing |
|---|---|---|---|
| ![Screenshot](/img/hh_report_main.png?raw=true "Screenshot") | ![Screenshot](/img/hh_report_includes.png?raw=true "Screenshot") | ![Screenshot](/img/hh_report_errors.png?raw=true "Screenshot") | ![Screenshot](/img/hh_report_missing.png?raw=true "Screenshot") |

## Building

I have used Jetbrains Rider to build the solution under `header_hero/header_hero.sln`. It is set to use .NET 9, so you might need to install that. Otherwise just build and run;
things Should Work, hopefully. A different C# IDE or some sort of command line build might work too, but I did not try that.
