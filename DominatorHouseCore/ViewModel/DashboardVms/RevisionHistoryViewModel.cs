using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace DominatorHouseCore.ViewModel.DashboardVms
{
    public class RevisionHistoryViewModel : BaseTabViewModel, IDashboardViewModel
    {
        public RevisionHistoryViewModel() : base("LangKeyRevisionHistory", "RevisionHistoryControlTemplate")
        {
            try
            {
                var text = string.Empty;
                using (var stream = Assembly.GetEntryAssembly().GetManifestResourceStream("DominatorHouse.RevisionHistory.revisionhistory.txt"))
                {
                    TextReader tr = new StreamReader(stream);
                    text = tr.ReadToEnd();
                }

                var result = Regex.Split(text, "Version").ToList();
                if (result.Count != 0)
                {
                    AddingVersionDetails(result);
                }
            }
            catch (System.Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddingVersionDetails(List<string> result)
        {
            ThreadFactory.Instance.Start(async () =>
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
                                Version = versionDatails[0],
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
