using System;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.IO;
using DominatorHouseCore.LogHelper;
using System.Linq;
using DominatorHouseCore.FileManagers;

namespace DominatorHouseCore.Models
{
    [ProtoContract]

    public class TemplateModel
    {
        public TemplateModel()
        {
            Id = Utilities.GetGuid(true);
        }



        [ProtoMember(1)]
        public string Id { get;  set; }

        [ProtoMember(2)]
        public string Name { get; set; }

        [ProtoMember(3)]
        public int CreationDate { get; set; }

        [ProtoMember(4)]
        public string ActivityType { get; set; }

        [ProtoMember(5)]
        public string ActivitySettings { get; set; }

        [ProtoMember(6)]
        public SocialNetworks SocialNetwork { get; set; }

        [ProtoMember(7)]
        public bool IsCampaignTemplate { get; set; }


        /// <summary>
        /// SaveTemplate is used to save the template in the template bin file.
        /// </summary>
        /// <typeparam name="T">Type of the class which is passed</typeparam>
        /// <param name="activitySettingObject">The value of the activity configuration settings</param>
        /// <param name="activityType">Its defined the activity base class</param>
        /// <param name="socialNetworks">Its defined it social networks such as FaceDom, GramDom and so on</param>
        /// <param name="templateName">Provide the template name from the client (the same as campaign name)</param>
        /// <returns>returns the template id</returns>
        public string SaveTemplate<T>(T activitySettingObject, string activityType, SocialNetworks socialNetworks, string templateName) where T : class
        {            
            try
            {                
                // serialize the object to string formate
                var activitySettings = Newtonsoft.Json.JsonConvert.SerializeObject(activitySettingObject);

                // Initialize and assign the values to TemplateModel for store in bin files
                TemplateModel newTemplate = new TemplateModel
                {
                    ActivityType = activityType,
                    ActivitySettings = activitySettings,
                    CreationDate = DateTime.Now.GetCurrentEpochTime(),
                    Name = templateName,
                    SocialNetwork = socialNetworks
                };


                // Serialize the template configuration to bin files
                TemplatesFileManager.Add(newTemplate);
                
                return newTemplate.Id;
            }

            catch (IOException ex)
            {
                GlobusLogHelper.log.Error($"Unable to add and save template [{templateName}] - {ex.Message}");
                ex.DebugLog();
                return null;
            }            
        }



    }

}
