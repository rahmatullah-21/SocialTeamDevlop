using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System.Windows;

namespace DominatorHouseCore.ViewModel
{
    public class RevisionHistoryViewModel : BindableBase
    {
        public RevisionHistoryViewModel()
        {
            //var fileContent = System.IO.File.ReadAllText(@"C:\Users\GLB-259\Desktop\version.txt").Trim();
           // var result = Regex.Split(fileContent, "version").ToList();
            //if (result != null && result.Count != 0)
            //{
            //    AddingVersionDetails(result);
            //}
        }

        private void AddingVersionDetails(List<string> result)
        {
            Task.Factory.StartNew(async () =>
            {
                result.ForEach(version =>
                {
                    List<string> LstContent = new List<string>();
                    var contentArray = version.ToString().Split('\r', '\n');

                    var data = contentArray.Select(line => line.EndsWith("\0") ? line.Replace("\0", "") : line).ToList();
                    if (!string.IsNullOrEmpty(data[0]))
                    {
                        var versionDatails = data[0].Split(',');
                        data.Skip(1).ForEach(item => { LstContent.Add(item); });
                        LstContent.RemoveAll(string.IsNullOrEmpty);

                        Application.Current.Dispatcher.Invoke(async () =>
                        {
                            _lstRevisionHistoryModel.Add(new RevisionHistoryModel
                            {
                                Version = _lstRevisionHistoryModel.Count + ". " + versionDatails[0],
                                RevisionDate = versionDatails[1],
                                LstContent = LstContent
                            });
                            await Task.Delay(1);
                        });

                    }

                });
                await Task.Delay(1);
            });
        }

        private RevisionHistoryModel _revisionHistoryModel = new RevisionHistoryModel();

        public RevisionHistoryModel RevisionHistoryModel
        {
            get
            {
                return _revisionHistoryModel;
            }
            set
            {
                if (_revisionHistoryModel == null & _revisionHistoryModel == value)
                    return;
                SetProperty(ref _revisionHistoryModel, value);
            }
        }
        private ObservableCollection<RevisionHistoryModel> _lstRevisionHistoryModel = new ObservableCollection<RevisionHistoryModel>();

        public ObservableCollection<RevisionHistoryModel> LstRevisionHistoryModel
        {
            get
            {
                return _lstRevisionHistoryModel;
            }
            set
            {
                if (_lstRevisionHistoryModel == null & _lstRevisionHistoryModel == value)
                    return;
                SetProperty(ref _lstRevisionHistoryModel, value);
            }
        }
    }
}
