using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

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


        public static List<TemplateModel> Get()
        {
            var result = new List<TemplateModel>();
            ApplyFunc(t =>
            {
                result.Add(t);
                return false;
            });

            return result;
        }

        public static TemplateModel GetTemplateById(string id)
        {
            var templates = Get();
            var result = templates.FirstOrDefault(t => t.Id == id);

            return result;
        }

        public static void Save(List<TemplateModel> templates)
        {
            BinFileHelper.UpdateTemplates(templates);
        }


        public static void Add(TemplateModel template) => BinFileHelper.Append(template);            

        // finds by id and delete
        public static void Delete(TemplateModel template)
        {
            var templates = Get();
            var toDelete = templates.FirstOrDefault(t => t.Id == template.Id);
            if (toDelete != null)
            {
                templates.Remove(toDelete);
                Save(templates);
            }
        }

        public static void Edit(TemplateModel template)
        {
            var templates = Get();
            var index = templates.FindIndex(t => t.Id == template.Id);
            if (index != -1)
            {
                templates[index] = template;
                Save(templates);
            }
        }
    }
}
