using CommonServiceLocator;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;

namespace DominatorHouseCore.FileManagers
{
    public static class PostFileManager
    {
        private static readonly IBinFileHelper BinFileHelper;

        static PostFileManager()
        {
            BinFileHelper = ServiceLocator.Current.GetInstance<IBinFileHelper>();
        }
        public static bool SavePost<T>(T post) where T : class
        {
            try
            {
                BinFileHelper.SavePosts(post);
                GlobusLogHelper.log.Debug("Post successfully saved");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static List<AddPostModel> GetAllPost()
        {
            return BinFileHelper.GetPostDetails();
        }


        public static void EditPost(AddPostModel post)
        {
            BinFileHelper.UpdatePost(post);
        }

        public static void Delete(Predicate<AddPostModel> match)
        {
            var posts = BinFileHelper.GetPostDetails();
            posts.RemoveAll(match);
            BinFileHelper.UpdateAllPosts(posts);
        }

    }
}
