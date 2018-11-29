using CommonServiceLocator;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FacebookModel = DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.FacebookModel;

namespace DominatorHouseCore.FileManagers
{
    public class GenericFileManager
    {
        private static readonly IProtoBuffBase ProtoBuffBase;

        static GenericFileManager()
        {
            ProtoBuffBase = ServiceLocator.Current.GetInstance<IProtoBuffBase>();
        }

        /// <summary>
        /// To holds the lock for specific file types
        /// </summary>
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

        /// <summary>
        /// To get the file details for given bin files
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="filePath">file path</param>
        /// <returns></returns>
        public static List<T> GetModuleDetails<T>(string filePath) where T : class
            => File.Exists(filePath) ? ProtoBuffBase.DeserializeList<T>(filePath) : new List<T>();


        /// <summary>
        /// To Update the details of the files
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="detailsList">List of file details</param>
        /// <returns></returns>
        public static bool UpdateModuleDetails<T>(List<T> detailsList) where T : class
        {
            try
            {
                // Fetch the file path from lock with type object
                return WithFile<T, bool>(file =>
                {
                    // serialize the file
                    bool result = ProtoBuffBase.SerializeList(detailsList, file);
                    GlobusLogHelper.log.Debug("Details successfully saved");
                    return result;
                });
            }
            catch (Exception ex)
            {

                ex.DebugLog();
                return false;
            }
        }

        /// <summary>
        /// To save all the details of the give object to respective file type's objects
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="lstModel">List of objects which is present after saved in bin file</param>
        internal static void SaveAll<T>(List<T> lstModel) where T : class
        {
            UpdateModuleDetails(lstModel);
            GlobusLogHelper.log.Debug("Details successfully saved");
        }

        /// <summary>
        /// To save the details with given file 
        /// </summary>
        /// <typeparam name="T">Targer type</typeparam>
        /// <param name="lstModel">List of details</param>
        /// <param name="file">saving file path</param>
        internal static void SaveAll<T>(List<T> lstModel, string file) where T : class
        {
            UpdateModuleDetails(lstModel, file);
            GlobusLogHelper.log.Debug("Details successfully saved");
        }

        /// <summary>
        /// Save the details to specified file
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="model">Details</param>
        /// <param name="file">file path</param>
        /// <returns></returns>
        internal static bool Save<T>(T model, string file) where T : class
        {
            try
            {

                using (var stream = File.Create(file))
                {
                    // Call for serialize
                    Serializer.Serialize(stream, model);
                    GlobusLogHelper.log.Debug("Details successfully saved");
                    return true;
                }

            }
            catch (Exception ex)
            {

                ex.DebugLog();
                return false;
            }

        }

        /// <summary>
        /// To Fetch the details from file
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="filePath">file path</param>
        /// <returns></returns>
        public static T GetModel<T>(string filePath) where T : class, new()
        {
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open))
                {
                    // Call for deserialize
                    return Serializer.Deserialize<T>(stream);
                }
            }
            catch (Exception ex)
            {

                ex.DebugLog();
                return new T();
            }

        }

        /// <summary>
        /// To Update the details to specified files
        /// </summary>
        /// <typeparam name="T">Targer Type</typeparam>
        /// <param name="detailsList">Detail list</param>
        /// <param name="file">file path</param>
        /// <returns></returns>
        public static bool UpdateModuleDetails<T>(List<T> detailsList, string file) where T : class
        {
            try
            {
                // Call for serialize
                var result = ProtoBuffBase.SerializeList(detailsList, file);
                GlobusLogHelper.log.Debug("Details successfully saved");
                return result;
            }
            catch (Exception ex)
            {

                ex.DebugLog();
                return false;
            }
        }


        /// <summary>
        /// Add multiple type values to file
        /// </summary>
        /// <typeparam name="T">Targer Type</typeparam>
        /// <param name="moduleToSave">type values</param>
        /// <param name="filePath">file path</param>
        /// <returns></returns>
        internal static bool AddRangeModule<T>(List<T> moduleToSave, string filePath) where T : class
        {
            try
            {
                moduleToSave.ForEach(x =>
                {
                    // Call for append the details 
                    ProtoBuffBase.AppendObject(x, filePath);
                });
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        /// <summary>
        /// To add the details to file path
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="moduleToSave">Module Details</param>
        /// <param name="filePath">file path</param>
        /// <returns></returns>
        internal static bool AddModule<T>(T moduleToSave, string filePath) where T : class
        {
            try
            {
                //Call for Append
                ProtoBuffBase.AppendObject<T>(moduleToSave, filePath);
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        /// <summary>
        /// To delete the details which matches the predicate condition
        /// </summary>
        /// <typeparam name="T">target type</typeparam>
        /// <param name="match">match condition</param>
        /// <param name="filePath">file path</param>
        public static void Delete<T>(Predicate<T> match, string filePath) where T : class
        {
            // Get all the details from file
            var moduleDetails = GetModuleDetails<T>(filePath);
            // Remove all matches
            moduleDetails.RemoveAll(match);
            // Update the bin files
            UpdateModuleDetails<T>(moduleDetails, filePath);
        }

        /// <summary>
        /// To delete the bin file
        /// </summary>
        /// <param name="filepath">file path</param>
        /// <returns></returns>
        public static bool DeleteBinFiles(string filepath)
        {
            try
            {
                // Check whether file is there or not, if present delete
                if (File.Exists(filepath))
                    File.Delete(filepath);
            }
            catch (IOException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return !File.Exists(filepath);
        }

        /// <summary>
        /// To override the details with specified file
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="instance">details</param>
        /// <param name="filePath">file path</param>
        /// <returns></returns>
        public static bool Overrride<T>(T instance, string filePath) where T : class
        {
            try
            {
                using (var stream = File.Create(filePath))
                {
                    // Call for serialize
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
        public static bool UpdateAdvancedSettingDetails<T>(List<T> detailsList, string fileType) where T : class
        {
            try
            {
                // Fetch the file path from lock with type object

                bool result = ProtoBuffBase.SerializeList(detailsList, fileType);
                GlobusLogHelper.log.Debug("Details successfully saved");
                return result;

            }
            catch (Exception ex)
            {

                ex.DebugLog();
                return false;
            }
        }
    }
}
