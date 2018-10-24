using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace DominatorHouseCore.FileManagers
{
    public class TemplatesFileManager
    {
        private static readonly ITemplatesCacheService TemplatesCacheService;

        static TemplatesFileManager()
        {
            TemplatesCacheService = IoC.Container.Resolve<ITemplatesCacheService>();
            TemplatesCacheService.GetTemplateModels();
        }
        // Updates Template with applying action to it and writes changes back to file
        public static void ApplyAction(Action<TemplateModel> actionToApply)
        {
            var templates = Get();

            foreach (var t in templates)
                actionToApply(t);

            TemplatesCacheService.UpsertTemplates(templates.ToArray());
        }

        // Same as above, but Func must return true if file needs to be overwritten        
        public static void ApplyFunc(Func<TemplateModel, bool> funcToApply)
        {
            var templates = Get();
            bool updated = false;

            foreach (var t in templates)
                updated |= funcToApply(t);

            if (updated)
                TemplatesCacheService.UpsertTemplates(templates.ToArray());
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
            return TemplatesCacheService.GetTemplateModels().ToList();
        }

        public static TemplateModel GetTemplateById(string id)
        {
            var templates = Get();
            var result = templates.FirstOrDefault(t => t.Id == id);

            return result;
        }

        public static void Save(List<TemplateModel> templates)
        {
            TemplatesCacheService.UpsertTemplates(templates.ToArray());
        }


        public static void Add(TemplateModel template) => TemplatesCacheService.UpsertTemplates(template);

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
        public static void Delete(Func<TemplateModel, bool> match)
        {
            var templates = Get();
            TemplatesCacheService.Delete(templates.Where(match).ToArray());
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
