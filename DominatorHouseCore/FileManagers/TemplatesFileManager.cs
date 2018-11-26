using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.FileManagers
{
    public interface ITemplatesFileManager
    {
        void ApplyAction(Action<TemplateModel> actionToApply);
        void ApplyFunc(Func<TemplateModel, bool> funcToApply);
        void ApplyActionForId(string templateId, Action<TemplateModel> actionToApply);
        void UpdateActivitySettings(string templateId, string activitySettingsJson);
        List<TemplateModel> Get();
        TemplateModel GetTemplateById(string id);
        void Save(List<TemplateModel> templates);
        void Add(TemplateModel template);
        void Delete(TemplateModel template);
        void Delete(Func<TemplateModel, bool> match);
        void Edit(TemplateModel template);
    }

    public class TemplatesFileManager : ITemplatesFileManager
    {
        private readonly ITemplatesCacheService _templatesCacheService;

        public TemplatesFileManager(ITemplatesCacheService cacheService)
        {
            _templatesCacheService = cacheService;
            _templatesCacheService.GetTemplateModels();
        }
        // Updates Template with applying action to it and writes changes back to file
        public void ApplyAction(Action<TemplateModel> actionToApply)
        {
            var templates = Get();

            foreach (var t in templates)
                actionToApply(t);

            _templatesCacheService.UpsertTemplates(templates.ToArray());
        }

        // Same as above, but Func must return true if file needs to be overwritten        
        public void ApplyFunc(Func<TemplateModel, bool> funcToApply)
        {
            var templates = Get();
            bool updated = false;

            foreach (var t in templates)
                updated |= funcToApply(t);

            if (updated)
                _templatesCacheService.UpsertTemplates(templates.ToArray());
        }

        public void ApplyActionForId(string templateId, Action<TemplateModel> actionToApply)
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

        public void UpdateActivitySettings(string templateId, string activitySettingsJson)
        {
            ApplyActionForId(templateId, t => t.ActivitySettings = activitySettingsJson);
        }


        public List<TemplateModel> Get()
        {
            return _templatesCacheService.GetTemplateModels().ToList();
        }

        public TemplateModel GetTemplateById(string id)
        {
            var templates = Get();
            var result = templates.FirstOrDefault(t => t.Id == id);

            return result;
        }

        public void Save(List<TemplateModel> templates)
        {
            _templatesCacheService.UpsertTemplates(templates.ToArray());
        }


        public void Add(TemplateModel template) => _templatesCacheService.UpsertTemplates(template);

        // finds by id and delete
        public void Delete(TemplateModel template)
        {
            var templates = Get();
            var toDelete = templates.FirstOrDefault(t => t.Id == template.Id);
            if (toDelete != null)
            {
                templates.Remove(toDelete);
                Save(templates);
            }
        }
        public void Delete(Func<TemplateModel, bool> match)
        {
            var templates = Get();
            _templatesCacheService.Delete(templates.Where(match).ToArray());
        }

        public void Edit(TemplateModel template)
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
