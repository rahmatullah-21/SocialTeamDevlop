namespace DominatorHouseCore.Interfaces.SocioPublisher
{
    public interface IGdPostSettings
    {
        string PostTitle { get; set; }

        bool IsPostAsStoryPost { get; set; }

        bool IsDeletePostAfterHours { get; set; }

        int DeletePostAfterHours { get; set; }

        bool IsGeoLocation { get; set; }

        string GeoLocationList { get; set; }

        bool IsTagUser { get; set; }

        string TagUserList { get; set; }
        bool IsGeoLocationName { get; set; }

        bool IsGeoLocationId { get; set; }

    }
}