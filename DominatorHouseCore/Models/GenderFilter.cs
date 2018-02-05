using ProtoBuf;

namespace DominatorHouseCore.Models
{
    /// <summary>
    /// Gender Filter is used to filter the user by genders
    /// </summary>

    [ProtoContract]
    public class GenderFilter
    {
        [ProtoMember(1)]
        // Is filter by gender required
        public bool IsFilterByGender { get; set; }


        [ProtoMember(3)]
        // Ignore the female user 
        public bool IgnoreFemalesUser { get; set; }


        [ProtoMember(2)]
        // Ignore the male user 
        public bool IgnoreMalesUser { get; set; }


        [ProtoMember(4)]
        // Ignore the other user 
        public bool IgnoreOthersUser { get; set; }


    }
}