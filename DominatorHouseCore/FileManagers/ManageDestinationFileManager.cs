using CommonServiceLocator;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DominatorHouseCore.FileManagers
{
    public interface IManageDestinationFileManager
    {
        List<PublisherManageDestinationModel> GetAll();
        void UpdateDestinations(IList<PublisherManageDestinationModel> libraryDestinations);
        void DeleteSelected(List<PublisherManageDestinationModel> accs);
        void Delete(Predicate<PublisherManageDestinationModel> match);
    }
    public class ManageDestinationFileManager : IManageDestinationFileManager
    {
        private readonly IBinFileHelper _binFileHelper;

        public ManageDestinationFileManager(IBinFileHelper binFileHelper)
        {
            //BinFileHelper = ServiceLocator.Current.GetInstance<IBinFileHelper>();
            _binFileHelper = binFileHelper;
        }

        #region private methods
        // Saves all destinations. Have to work Only in Social library. Otherwise use UpdateDestinations() method to update PublisherDestinations.bin
        // NOTE: make sure lstPublisherDetails contains all destinations
        private void SaveAll(List<PublisherManageDestinationModel> lstPublisherDetails)
        {
            // Warning: make sure lstPublisherDetails contains all publisher            
            _binFileHelper.UpdateAllManageDestination(lstPublisherDetails);
            GlobusLogHelper.log.Debug($"{lstPublisherDetails.Count} Destination successfully saved");
        }
        // Same as above, but Func must return true if file needs to be overwritten        
       
        #endregion

        #region  public methods

        public List<PublisherManageDestinationModel> GetAll()
        {
            return _binFileHelper.GetPublisherManageDestinationModels();
        }

        // Update publisher entries and save to PublisherDestinations.bin        
        public void UpdateDestinations(IList<PublisherManageDestinationModel> libraryDestinations)
        {
            //To DO: Change this on GetAll()
            var all = _binFileHelper.GetPublisherManageDestinationModels();

            // Update all entries that exists in libraryDestinations, and add that does not exists
            for (int i = 0; i < libraryDestinations.Count; i++)
            {
                var acc = libraryDestinations[i];
                var ix = all.FindIndex(a => acc.DestinationId == a.DestinationId);
                if (ix == -1)
                    all.Add(acc);
                else
                    all[ix] = acc;
            }
            _binFileHelper.UpdateAllManageDestination(all);
        }
        public void ApplyFunc(Func<PublisherManageDestinationModel, bool> funcToApply)
        {
            bool updated = false;
            //To DO: Change this on GetAll()
            var destinations = _binFileHelper.GetPublisherManageDestinationModels();

            foreach (var a in destinations)
                updated |= funcToApply(a);

            if (updated)
                _binFileHelper.UpdateAllManageDestination(destinations);
        }
        public void FillList<T>(ObservableCollection<T> destinationList) where T : class
        {
            destinationList.Clear();
            ApplyFunc(a =>
            {
                destinationList.Add((T)(object)a);
                return false;
            });
        }


        public PublisherManageDestinationModel GetByDestinationId(string destinationId)
        {
            var lst = GetAll();
            return lst.FirstOrDefault(x => x.DestinationId == destinationId);
        }

        public bool Add(PublisherManageDestinationModel account)
        {
            var lst = GetAll() ?? new List<PublisherManageDestinationModel>();
            lst.Add(account);
            _binFileHelper.UpdateAllManageDestination(lst);
            return true;
        }

        public bool AddRange(List<PublisherManageDestinationModel> destinationList)
        {
            var lst = GetAll() ?? new List<PublisherManageDestinationModel>();
            lst.AddRange(destinationList);
            _binFileHelper.UpdateAllManageDestination(lst);
            return true;
        }

        public void DeleteSelected(List<PublisherManageDestinationModel> accs)
        {
            var all = GetAll().Where(a => accs.FirstOrDefault(p => p.DestinationId == a.DestinationId) == null).ToList();
            SaveAll(all);
        }

        public void Delete(Predicate<PublisherManageDestinationModel> match)
        {
            var accs = GetAll();
            accs.RemoveAll(match);
            _binFileHelper.UpdateAllAccounts(accs);
        }
        #endregion

    }
}