namespace DominatorHouseCore.Interfaces
{
    public interface IComments
    {
        string CommentId { get; set; }

        string CommenterID { get; set; }

        string PostId { get; set; }
    }
}