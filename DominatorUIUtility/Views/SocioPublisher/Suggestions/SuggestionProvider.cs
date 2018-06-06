using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorUIUtility.Views.SocioPublisher.CustomControl;

namespace DominatorUIUtility.Views.SocioPublisher.Suggestions
{
    public class SuggestionProvider : ISuggestionProvider
    {
        public IEnumerable<SocinatorIntellisenseModel> ListOfMacros { get; set; }

        public IEnumerable GetSuggestions(string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return null;

            if (!filter.StartsWith("{") && !filter.EndsWith("}"))
                return null;

            return ListOfMacros.Where(x => x.Key.ToLower().Contains(filter.ToLower()));
        }

        public SuggestionProvider()
        {
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{Hello}", Value = "Hello" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{Globussoft}", Value = "Globussoft" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{DotNet}", Value = "Globussoft" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{Android}", Value = "Globussoft" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{Hello}", Value = "Hello" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{Globussoft}", Value = "Globussoft" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{DotNet}", Value = "Globussoft" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{Android}", Value = "Globussoft" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{Hello}", Value = "Hello" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{Globussoft}", Value = "Globussoft" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{DotNet}", Value = "Globussoft" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{Hello}", Value = "Hello" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{Globussoft}", Value = "Globussoft" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{DotNet}", Value = "Globussoft" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{Android}", Value = "Globussoft" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{Hello}", Value = "Hello" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{Globussoft}", Value = "Globussoft" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{DotNet}", Value = "Globussoft" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{Android}", Value = "Globussoft" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{Hello}", Value = "Hello" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{Globussoft}", Value = "Globussoft" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{DotNet}", Value = "Globussoft" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{Android}", Value = "Globussoft" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{Android}", Value = "Globussoft" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{Hello}", Value = "Hello" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{Globussoft}", Value = "Globussoft" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{DotNet}", Value = "Globussoft" });
            SocinatorInitialize.Macros.Add(new SocinatorIntellisenseModel { Key = "{Android}", Value = "Globussoft" });
            ListOfMacros = SocinatorInitialize.Macros;
        }
    }
}