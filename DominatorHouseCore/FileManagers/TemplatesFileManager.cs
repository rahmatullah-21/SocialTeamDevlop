using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.FileManagers
{
    public class TemplatesFileManager
    {
        // Updates Template with applying action to it and writes changes back to file
        public static void ApplyAction(Action<TemplateModel> actionToApply)
        {
            var templates = BinFileHelper.GetTemplateDetails().ToList();

            foreach (var t in templates)
                actionToApply(t);

            BinFileHelper.UpdateTemplates(templates);
        }

        // Same as above, but Func must return true if file needs to be overwritten        
        public static void ApplyFunc(Func<TemplateModel, bool> funcToApply)
        {
            var templates = BinFileHelper.GetTemplateDetails().ToList();
            bool updated = false;

            foreach (var t in templates)
                updated |= funcToApply(t);

            if (updated)
                BinFileHelper.UpdateTemplates(templates);
        }

        public static void ApplyActionForId(string templateId, Action<TemplateModel> actionToApply)
        {
            ApplyFunc(t =>
            {
                if (t.Id == templateId)
                {
                    actionToApply(t);
                    return true;
                }
                else
                    return false;
            });
        }

        public static void UpdateActivitySettings(string templateId, string activitySettingsJson)
        {
            ApplyActionForId(templateId, t => t.ActivitySettings = activitySettingsJson);
        }
    }
}
