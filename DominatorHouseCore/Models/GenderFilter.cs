using System;
using System.Collections.Generic;
using DominatorHouseCore.Enums;
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

    public static class GenderGuesser
    {
        private static readonly HashSet<string> FemaleNames = new HashSet<string>((IEnumerable<string>)Resources.femaleNames.Split(new string[1]
        {
            Environment.NewLine
        }, StringSplitOptions.RemoveEmptyEntries));
        private static readonly HashSet<string> MaleNames = new HashSet<string>((IEnumerable<string>)Resources.maleNames.Split(new string[1]
        {
            Environment.NewLine
        }, StringSplitOptions.RemoveEmptyEntries));
        private static readonly HashSet<string> UnisexNames = new HashSet<string>((IEnumerable<string>)Resources.unisexNames.Split(new string[1]
        {
            Environment.NewLine
        }, StringSplitOptions.RemoveEmptyEntries));

        public static Gender GetGender(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name can't be empty");
            name = name.ToLower();
            if (GenderGuesser.MaleNames.Contains(name))
                return Gender.Male;
            if (GenderGuesser.FemaleNames.Contains(name))
                return Gender.Female;
            return !GenderGuesser.UnisexNames.Contains(name) ? Gender.Unknown : Gender.Unisex;
        }
    }
}