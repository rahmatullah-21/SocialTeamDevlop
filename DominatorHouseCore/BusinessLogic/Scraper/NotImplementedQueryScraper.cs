using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using DominatorHouseCore.Process;

namespace DominatorHouseCore.BusinessLogic.Scraper
{
    /// <summary>
    /// Stub class for libraries where scraper does not implemented or not used.    
    /// </summary>
    public class NotImplementedQueryScraper : AbstractQueryScraper
    {
        public NotImplementedQueryScraper(JobProcess jobProcess) : base(jobProcess)
        {
        }

        protected override void ScrapeToUnfollow()
        {
            throw new NotImplementedException();
        }

        protected override void StartProcessForCustomUsers(QueryInfo queryInfo)
        {
            throw new NotImplementedException();
        }

        protected override void StartProcessForFollowersOfFollowers(QueryInfo queryInfo)
        {
            throw new NotImplementedException();
        }

        protected override void StartProcessForFollowersOfFollowings(QueryInfo queryInfo)
        {
            throw new NotImplementedException();
        }

        protected override void StartProcessForMediaCommenters(QueryInfo queryInfo)
        {
            throw new NotImplementedException();
        }

        protected override void StartProcessForMediaLikers(QueryInfo queryInfo)
        {
            throw new NotImplementedException();
        }

        protected override void StartProcessForSomeonesFollowers(QueryInfo queryInfo)
        {
            throw new NotImplementedException();
        }

        protected override void StartProcessForSomeonesFollowings(QueryInfo queryInfo)
        {
            throw new NotImplementedException();
        }

        protected override void StartProcessForSuggestedusers(QueryInfo queryInfo)
        {
            throw new NotImplementedException();
        }

        protected override void StartProcessWithCustomPhotos(QueryInfo queryInfo)
        {
            throw new NotImplementedException();
        }

        protected override void StartProcessWithHashtagPosts(QueryInfo queryInfo)
        {
            throw new NotImplementedException();
        }

        protected override void StartProcessWithHashtagUsers(QueryInfo queryInfo)
        {
            throw new NotImplementedException();
        }

        protected override void StartProcessWithKeyword(QueryInfo queryInfo)
        {
            throw new NotImplementedException();
        }

        protected override void StartProcessWithLocationPosts(QueryInfo queryInfo)
        {
            throw new NotImplementedException();
        }

        protected override void StartProcessWithLocationUsers(QueryInfo queryInfo)
        {
            throw new NotImplementedException();
        }
    }
}
