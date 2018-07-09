using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore.Diagnostics;

namespace DominatorHouseCore.Utility
{
    public static class MacrosHelper
    {
        private static readonly Regex MacrosParser = new Regex(@"{[^}]*}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string SubstituteMacroValues(string text)
        {
            SocinatorInitialize.InitializeMacros();           
            return MacrosParser.Replace(text, MacrosMatchEvaluator);             
        }

        private static string MacrosMatchEvaluator(Match match)
        {
            var matchReplaceValue = SocinatorInitialize.Macros.FirstOrDefault(x => x.Key == match.Value)?.Value;
            return string.IsNullOrEmpty(matchReplaceValue) ? match.Value : matchReplaceValue;
        }

        public static List<string> GetAvailableMacroLists(string text)
        {
            var matches = MacrosParser.Matches(text);
            return matches.Count==0 ? new List<string>() : (from Match match in matches select match.Value).ToList();
        }
    }
}