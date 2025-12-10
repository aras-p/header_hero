using System.Collections.Generic;
using System.Linq;

namespace HeaderHero.Parser;

public class ItemAnalytics
{
    public readonly HashSet<string> AllIncludes = [];
    public int TotalIncludeLines;
    public readonly HashSet<string> AllIncludedBy = [];
    public readonly HashSet<string> TranslationUnitsIncludedBy = [];
    public bool Analyzed;
}

public class Analytics
{
    public readonly Dictionary<string, ItemAnalytics> Items = new();

    public static Analytics Analyze(Data.Project project)
    {
        Analytics analytics = new Analytics();
        foreach (var kvp in project.Files)
            analytics.Analyze(kvp.Key, project);
        return analytics;
    }

    ItemAnalytics Analyze(string path, Data.Project project)
    {
        if (!Items.TryGetValue(path, out var a))
        {
            a = new ItemAnalytics();
            Items.Add(path, a);
        }
        if (a.Analyzed)
            return a;
        a.Analyzed = true;

        Data.SourceFile sf = project.Files[path];
        foreach (string include in sf.AbsoluteIncludes)
        {
            if (include == path)
                continue;

            bool is_tu = Data.SourceFile.IsTranslationUnitPath(path);

            ItemAnalytics ai = Analyze(include, project);
            a.AllIncludes.Add(include);
            ai.AllIncludedBy.Add(path);
            if (is_tu)
                ai.TranslationUnitsIncludedBy.Add (path);


            a.AllIncludes.UnionWith(ai.AllIncludes);
            foreach (string inc in ai.AllIncludes) {
                Items[inc].AllIncludedBy.Add(path);
                if (is_tu)
                    Items[inc].TranslationUnitsIncludedBy.Add (path);
            }
        }

        a.TotalIncludeLines = a.AllIncludes.Sum(f => project.Files[f].Lines);
        return a;
    }
}