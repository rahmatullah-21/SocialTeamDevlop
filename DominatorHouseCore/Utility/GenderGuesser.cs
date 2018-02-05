using DominatorHouseCore.Enums;
using System;
using System.Collections.Generic;
using DominatorHouseCore;

namespace DominatorHouse.UsefullUtilitiesLibrary
{
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
