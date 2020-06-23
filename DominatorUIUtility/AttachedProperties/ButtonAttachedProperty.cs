using DominatorHouseCore.Enums;

namespace DominatorUIUtility.AttachedProperties
{
    /// <summary>
    /// The IsBusy attached property for a anything that wants to flag if the control is busy
    /// </summary>
    public class IsBusyProperty : BaseAttachedProperty<IsBusyProperty, bool>
    {
    }

    // <summary>
    /// The Iselected attached property for a anything that wants to flag if the control is busy
    /// </summary>
    public class SelectedTabProperty : BaseAttachedProperty<SelectedTabProperty, SelectedMenuItem>
    {
    }

    // <summary>
    /// The Iselected attached property for a anything that wants to flag if the control is busy
    /// </summary>
    public class SelectedTabItemNameProperty : BaseAttachedProperty<SelectedTabItemNameProperty, string>
    {
    }
}
