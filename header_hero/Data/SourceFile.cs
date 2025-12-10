using System.Collections.Generic;

namespace HeaderHero.Data;

public class SourceFile
{
	public List<string> LocalIncludes { get; init; } = [];
	public List<string> SystemIncludes { get; init; } = [];
	public List<string> AbsoluteIncludes { get; set; } = [];
	public int Lines { get; init; }
	public bool Precompiled { get; init; }

	public static bool IsTranslationUnitExtension(string ext)
	{
		return ext is ".cpp" or ".c" or ".cc" or ".cxx" or ".mm" or ".m";
	}
		
	public static bool IsTranslationUnitPath(string path)
	{
		return IsTranslationUnitExtension(System.IO.Path.GetExtension(path));
	}
}