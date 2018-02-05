using System;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using ProtoBuf;

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
        /// <param name="templateName">Provide the template name from the client</param>
        /// <returns>returns the template id</returns>
        public string SaveTemplate<T>(T activitySettingObject, string activityType, SocialNetworks socialNetworks, string templateName) where T : class
        {
            TemplateModel templateModel = null;
            try
            {
                // serialize the object to string formate
                var activitySettings = Newtonsoft.Json.JsonConvert.SerializeObject(activitySettingObject);

                //Initialize and assign the values to TemplateModel for store in bin files
                 templateModel = new TemplateModel
                {
                    ActivityType = activityType,
                    ActivitySettings = activitySettings,
                    CreationDate = DateTime.Now.GetCurrentEpochTime(),
                    Name = templateName,
                    SocialNetwork = socialNetworks
                };

                // Get the path for current social network
                var socialNetworkPath = new SocialNetworkPathFactory(socialNetworks).GetSocialNetworkConfigPath();

                DirectoryUtilities.CreateDirectory(socialNetworkPath);

                // Serialize the template configuration to bin files
                ProtoBuffBase.SerializeObjects(templateModel, $"{socialNetworkPath}\\{ConstantVariable.TemplateBinName}");

            }
            catch (Exception)
            {

                return null;
            }
            return $"{templateModel.Id}";

        }



    }

}
