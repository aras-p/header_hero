using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeaderHero.Parser
{
    public class ItemAnalytics
    {
        public HashSet<string> AllIncludes;
        public int TotalIncludeLines;
        public HashSet<string> AllIncludedBy;
		public HashSet<string> TranslationUnitsIncludedBy;
        public bool Analyzed;

        public ItemAnalytics()
        {
            AllIncludes = new HashSet<string>();
            TotalIncludeLines = 0;
            AllIncludedBy = new HashSet<string>();
			TranslationUnitsIncludedBy = new HashSet<string>();
            Analyzed = false;
        }
    }

    public class Analytics
    {
        public Dictionary<string, ItemAnalytics> Items = new Dictionary<string,ItemAnalytics>();

        static public Analytics Analyze(Data.Project project)
        {
            Analytics analytics = new Analytics();
            foreach (var kvp in project.Files)
                analytics.Analyze(kvp.Key, project);
            return analytics;
        }

        private ItemAnalytics Analyze(string path, Data.Project project)
        {
            Data.SourceFile sf = project.Files[path];
            if (!Items.ContainsKey(path))
                Items[path] = new ItemAnalytics();
            ItemAnalytics a = Items[path];
            if (a.Analyzed)
                return a;
            a.Analyzed = true;

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
}
