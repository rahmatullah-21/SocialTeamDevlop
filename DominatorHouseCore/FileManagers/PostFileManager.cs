using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.FileManagers
{
    public static class PostFileManager
    {
        public static bool SavePost<T>(T post) where T : class
        {
            try
            {
                BinFileHelper.SavePosts(post);
                GlobusLogHelper.log.Debug($"Post successfully saved");
                return true;
            }
            catch (Exception )
            {
                return false;
            }
            return false;
        }
        public static List<AddPostModel> GetAllPost()
        {
            return BinFileHelper.GetPostDetails<AddPostModel>();
        }


        public static void EditPost<TModel>(TModel post) where TModel : class => BinFileHelper.UpdatePost(post);

        public static void Delete<PModel>(Predicate<PModel> match) where PModel : class
        {
            var posts = BinFileHelper.GetPostDetails<PModel>();
            var toDelete = posts.FindAll(match);
            posts.RemoveAll(match);
            BinFileHelper.UpdateAllPosts(posts);
        }

    }
}
