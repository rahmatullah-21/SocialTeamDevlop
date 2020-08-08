using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DominatorHouseCore.BusinessLogic.Scraper
{
    // Interface that have to be implemented by certain social network scraper
    // InstagramScraper, PinterestScraper, TwitterScrappr etc.
    public abstract class AbstractQueryScraper
    {
        private readonly Dictionary<UserQueryParameters, Action<QueryInfo>> _queryToActionTable;

        private JobProcess _jobProcess;

        protected AbstractQueryScraper(JobProcess jobProcess)
        {
            _jobProcess = jobProcess;
            Debug.Assert(_jobProcess != null);

            // NOTE: add new action associated with new QueryType if needed
            _queryToActionTable = new Dictionary<UserQueryParameters, Action<QueryInfo>>() {
                { UserQueryParameters.HashtagPost, StartProcessWithHashtagPosts },
                { UserQueryParameters.HashtagUsers, StartProcessWithHashtagUsers },
                { UserQueryParameters.Keywords, StartProcessWithKeyword },
                { UserQueryParameters.SomeonesFollowers, StartProcessForSomeonesFollowers },
                { UserQueryParameters.SomeonesFollowings, StartProcessForSomeonesFollowings },
                { UserQueryParameters.FollowersOfFollowings, StartProcessForFollowersOfFollowings },
                { UserQueryParameters.FollowersOfFollowers, StartProcessForFollowersOfFollowers },
                { UserQueryParameters.LocationUsers, StartProcessWithLocationUsers },
                { UserQueryParameters.LocationPosts, StartProcessWithLocationPosts },
                { UserQueryParameters.CustomUsers, StartProcessForCustomUsers },
                { UserQueryParameters.SuggestedUsers, StartProcessForSuggestedusers },
                { UserQueryParameters.CustomPhotos, StartProcessWithCustomPhotos },
                { UserQueryParameters.UsersWhoLikedPost, StartProcessForMediaLikers },
                { UserQueryParameters.UsersWhoCommentedOnPost, StartProcessForMediaCommenters },
                { UserQueryParameters.BoardFollowers, StartProcessForBoardFollowers },
                { UserQueryParameters.CustomBoard, StartProcessForCustomBoards },
                { UserQueryParameters.CustomPin, StartProcessForCustomPin },
                { UserQueryParameters.NewsFeedPins, StartProcessForNewsFeedPins },
                { UserQueryParameters.FromSomeonesCircle, StartProcessForSomeonesCircle}

            };
        }
      
        protected virtual void StartProcessForSomeonesCircle(QueryInfo queryInfo) { }
        protected virtual void StartProcessForBoardFollowers(QueryInfo queryInfo) { }
        protected virtual void StartProcessForCustomBoards(QueryInfo queryInfo) { }
        protected virtual void StartProcessForCustomPin(QueryInfo queryInfo) { }
        protected virtual void StartProcessForNewsFeedPins(QueryInfo queryInfo) { }
        protected virtual void StartProcessWithKeyword(QueryInfo queryInfo) { }
        protected virtual void StartProcessWithHashtagUsers(QueryInfo queryInfo) { }
        protected virtual void StartProcessWithHashtagPosts(QueryInfo queryInfo) { }
        protected virtual void StartProcessForSomeonesFollowers(QueryInfo queryInfo) { }
        protected virtual void StartProcessForSomeonesFollowings(QueryInfo queryInfo) { }
        protected virtual void StartProcessForFollowersOfFollowers(QueryInfo queryInfo) { }
        protected virtual void StartProcessForFollowersOfFollowings(QueryInfo queryInfo) { }
        protected virtual void StartProcessWithLocationUsers(QueryInfo queryInfo) { }
        protected virtual void StartProcessWithLocationPosts(QueryInfo queryInfo) { }
        protected virtual void StartProcessForCustomUsers(QueryInfo queryInfo) { }
        protected virtual void StartProcessForSuggestedusers(QueryInfo queryInfo) { }
        protected virtual void StartProcessWithCustomPhotos(QueryInfo queryInfo) { }
        protected virtual void StartProcessForMediaLikers(QueryInfo queryInfo) { }
        protected virtual void StartProcessForMediaCommenters(QueryInfo queryInfo) { }


        // Call this method inside override methods of derived class for ignored queries
        protected void Ignore(QueryInfo queryInfo) =>
            GlobusLogHelper.log.Info($"Scrape for '{queryInfo.QueryType}' query ignored");
    }
}

