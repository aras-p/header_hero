using System.Collections.Generic;

namespace HeaderHero.Data;

public class SourceFile
{
	public List<string> LocalIncludes { get; set; } = [];
	public List<string> SystemIncludes { get; set; } = [];
	public List<string> AbsoluteIncludes { get; set; } = [];
	public int Lines { get; set; }
	public bool Touched { get; set; }
	public bool Precompiled { get; set; }

	static public bool IsTranslationUnitExtension(string ext)
	{
		return ext is ".cpp" or ".c" or ".cc" or ".cxx" or ".mm" or ".m";
	}
		
	public static bool IsTranslationUnitPath(string path)
	{
		return IsTranslationUnitExtension(System.IO.Path.GetExtension(path));
	}
}