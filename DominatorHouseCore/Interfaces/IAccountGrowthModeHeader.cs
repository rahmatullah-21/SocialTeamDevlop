using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Interfaces
{
    public interface IAccountGrowthModeHeader
    {
        ObservableCollectionBase<string> AccountItemSource { get; set; }

        string SelectedItem { get; set; }

    }
}