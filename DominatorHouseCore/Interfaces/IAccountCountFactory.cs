namespace DominatorHouseCore.Interfaces
{
    /// <summary>
    /// IAccountCountFactory is used to specify the account count details with their respective visiblity
    /// such as for instagram - FollowerCount (HeaderColumn2Value) , FollowingCount (HeaderColumn3Value) , PostCount(HeaderColumn4Value)
    /// NOTE : HeaderColumn1Value is reserved for only dominator
    /// </summary>
    public interface IAccountCountFactory
    {
        string HeaderColumn1Value { get; set; }

        bool HeaderColumn1Visiblity { get; set; }

        string HeaderColumn2Value { get; set; }

        bool HeaderColumn2Visiblity { get; set; }

        string HeaderColumn3Value { get; set; }

        bool HeaderColumn3Visiblity { get; set; }

        string HeaderColumn4Value { get; set; }

        bool HeaderColumn4Visiblity { get; set; }
    }
}