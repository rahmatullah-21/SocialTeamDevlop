using ProtoBuf;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class ManageReportVideosContentModel : ManageCommentModel
    {
        public int ReportOption { get; set; }
        public int ReportSubOption { get; set; }
        public int VideoTimestampPercentage { get; set; }
    }
}
