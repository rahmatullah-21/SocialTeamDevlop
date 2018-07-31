using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using ProtoBuf;
using FacebookModel = DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.FacebookModel;

namespace DominatorHouseCore.FileManagers
{
    public class GenericFileManager
    {
        private static Dictionary<Type, Tuple<object, Func<string>>> __lockAndFileByType =
          new Dictionary<Type, Tuple<object, Func<string>>>
          {
                {
                    typeof(CampaignDetails),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetIndexCampaignFile)
                },
              {
                  typeof(TemplateModel),
                  Tuple.Create(new object(), (Func<string>) ConstantVariable.GetTemplatesFile)
              },
                {
                    typeof(ProxyManagerModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetOtherProxyFile)
                },
              {
                  typeof(AddPostModel), Tuple.Create(new object(), (Func<string>) ConstantVariable.GetOtherPostsFile)
              },
              {
                  typeof(Configuration),
                  Tuple.Create(new object(), (Func<string>) ConstantVariable.GetOtherConfigFile)
              },

                //Todo: Following line need to delete
                {
                    typeof(PublisherAccountDetails),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherFile)
                },

                {
                    typeof(PublisherPostlistModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherCreatePostlistFolder)
                },

                {
                    typeof(PublisherManageDestinationModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherDestinationsFile)
                },
                {
                    typeof(PublisherCreateDestinationModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherCreateDestinationsFolder)
                },
                {
                    typeof(PublisherPostlistSettingsModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherPostlistSettingsFile)
                },
                {
                    typeof(CampaignInteractionViewModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetConfigurationDir)
                }
              ,
              {
                  typeof(FacebookModel),
                  Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherOtherConfigDir)
              }
              ,
              {
                  typeof(GeneralModel),
                  Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherOtherConfigDir)
              }
              ,
              {
                  typeof(GooglePlusModel),
                  Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherOtherConfigDir)
              }
              ,
              {
                  typeof(DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.InstagramModel),
                  Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherOtherConfigDir)
              } ,
              {
                  typeof(DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.PinterestModel),
                  Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherOtherConfigDir)
              },
              {
                  typeof(DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.TumblrModel),
                  Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherOtherConfigDir)
              },
              {
                  typeof(DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.TwitterModel),
                  Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherOtherConfigDir)
              },
              {
                  typeof(object),
                  Tuple.Create(new object(), (Func<string>) ConstantVariable.GetIndexAccountFile)
              }
          };

        /// <summary>
        /// Do something while locking the file that is the repository for the corresponding class
        /// </summary>
        /// <typeparam name="T">subject</typeparam>
        /// <typeparam name="R">return type</typeparam>
        /// <param name="act">action to perform</param>
        /// <returns>repeats the action returned value</returns>
        static R WithFile<T, R>(Func<string, R> act)
        {
            Tuple<object, Func<string>> typeConfig;
            // first, try the actual type
            if (!__lockAndFileByType.TryGetValue(typeof(T), out typeConfig))
            {
                // second, try to see if it's an assignable type
                var presentBaseClass = __lockAndFileByType.Keys.Except(new Type[] { typeof(object) }).FirstOrDefault(
                    candidateBase => candidateBase.IsAssignableFrom(typeof(T)));
                if (presentBaseClass == default(Type))
                {
                    presentBaseClass = typeof(object);
                }
                typeConfig = __lockAndFileByType[presentBaseClass];
            }
            lock (typeConfig.Item1)
            {
                return act(typeConfig.Item2());
            }
        }

        public static List<T> GetModuleDetails<T>(string filePath) where T : class
            => File.Exists(filePath) ? ProtoBuffBase.DeserializeList<T>(filePath) : new List<T>();



        public static bool UpdateModuleDetails<T>(List<T> detailsList) where T : class
        {
            try
            {
                return WithFile<T, bool>(file =>
                {
                    bool result = ProtoBuffBase.SerializeList(detailsList, file);
                    GlobusLogHelper.log.Debug("Details successfully saved");
                    return result;
                });
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Updation error - " + ex.Message);
                ex.DebugLog();
                return false;
            }
        }

        internal static void SaveAll<T>(List<T> lstModel) where T : class
        {
            UpdateModuleDetails(lstModel);
            GlobusLogHelper.log.Debug("Details successfully saved");
        }

        internal static void SaveAll<T>(List<T> lstModel, string file) where T : class
        {
            UpdateModuleDetails(lstModel, file);
            GlobusLogHelper.log.Debug("Details successfully saved");
        }
        internal static bool Save<T>(T model, string file) where T : class
        {
            try
            {

                using (var stream = File.Create(file))
                {
                    Serializer.Serialize(stream, model);
                    GlobusLogHelper.log.Debug("Details successfully saved");
                    return true;
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Saving error - " + ex.Message);
                ex.DebugLog();
                return false;
            }

        }

        public static T GetModel<T>(string filePath) where T : class, new()
        {
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open))
                {
                    return Serializer.Deserialize<T>(stream);
                }
            }
            catch (Exception ex)
            {

                ex.DebugLog();
                return new T();
            }

        }
        public static bool UpdateModuleDetails<T>(List<T> detailsList, string file) where T : class
        {
            try
            {
                var result = ProtoBuffBase.SerializeList(detailsList, file);
                GlobusLogHelper.log.Debug("Details successfully saved");
                return result;

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Updation error - " + ex.Message);
                ex.DebugLog();
                return false;
            }
        }



        internal static bool AddRangeModule<T>(List<T> moduleToSave, string filePath) where T : class
        {
            try
            {
                moduleToSave.ForEach(x =>
                {
                    ProtoBuffBase.AppendObject(x, filePath);
                });
                return true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error($"Error caught while adding the account " + ex.StackTrace);
                return false;
            }
        }

        internal static bool AddModule<T>(T moduleToSave, string filePath) where T : class
        {
            try
            {
                ProtoBuffBase.AppendObject<T>(moduleToSave, filePath);
                return true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error($"Error caught while adding the account " + ex.StackTrace);
                return false;
            }
        }
        public static void Delete<T>(Predicate<T> match, string filePath) where T : class
        {
            var moduleDetails = GetModuleDetails<T>(filePath);
            moduleDetails.RemoveAll(match);
            UpdateModuleDetails<T>(moduleDetails, filePath);
        }


        public static bool DeleteBinFiles(string filepath)
        {
            try
            {
                if (File.Exists(filepath))
                    File.Delete(filepath);
            }
            catch (IOException ex)
            {
                GlobusLogHelper.log.Error($"Unable to delete file {filepath} - {ex.Message}");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return !File.Exists(filepath);
        }

        public static bool Overrride<T>(T instance, string filePath) where T : class
        {
            try
            {
                using (var stream = File.Create(filePath))
                {
                    Serializer.Serialize(stream, instance);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }

        }

    }
}
