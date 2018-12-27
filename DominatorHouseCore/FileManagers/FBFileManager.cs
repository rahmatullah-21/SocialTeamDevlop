using System;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.FileManagers
{
    public interface IFBFileManager
    {
        bool SaveFacebookConfig(FacebookModel facebookModel);
        FacebookModel GetFacebookConfig();
    }
    public class FBFileManager : IFBFileManager
    {
        private readonly IProtoBuffBase _protoBuffBase;
        private readonly ILockFileConfigProvider _lockFileConfigProvider;
        private readonly IFileSystemProvider _fileSystemProvider;

        public FBFileManager(IProtoBuffBase protoBuffBase, ILockFileConfigProvider lockFileConfigProvider, IFileSystemProvider fileSystemProvider)
        {
            _protoBuffBase = protoBuffBase;
            _lockFileConfigProvider = lockFileConfigProvider;
            _fileSystemProvider = fileSystemProvider;
        }
        public bool SaveFacebookConfig(FacebookModel facebookModel)
        {
            try
            {
               return _lockFileConfigProvider.WithFile<FacebookModel, bool>(file =>
                {
                    using (var stream = _fileSystemProvider.Create(file))
                    {
                        Serializer.Serialize(stream, facebookModel);
                        GlobusLogHelper.log.Info("Details successfully saved");
                        return true;
                    }
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }
        public FacebookModel GetFacebookConfig()
        {
            FacebookModel facebookModel = new FacebookModel();
            try
            {
                _lockFileConfigProvider.WithFile<FacebookModel, bool>(file =>
                {

                    if (_fileSystemProvider.Exists(file))
                    {
                        facebookModel = _protoBuffBase.Deserialize<FacebookModel>(file);

                    }
                    return true;
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return facebookModel;
        }
    }
}
