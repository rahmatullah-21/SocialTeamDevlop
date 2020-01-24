using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Models.SocioPublisher;
using LegionUIUtility.Views.SocioPublisher.CustomControl;

namespace LegionUIUtility.Views.SocioPublisher.Suggestions
{
    public class SuggestionProvider : ISuggestionProvider
    {
        public IEnumerable<SocinatorIntellisenseModel> ListOfMacros { get; set; }

        public IEnumerable GetSuggestions(string filter)
        {
            SocinatorInitialize.InitializeMacros();
            ListOfMacros = SocinatorInitialize.Macros;

            if (string.IsNullOrEmpty(filter))
                return null;

            if (!filter.StartsWith("{") && !filter.EndsWith("}"))
                return null;

            return ListOfMacros.Where(x => x.Key.ToLower().Contains(filter.ToLower()));
        }
    }
}