using AutoMapper;
using System.Collections.Generic;

namespace Legion.AutoMapping
{
    public static class AutoMapperConfiguration
    {
        public static void Init(IEnumerable<Profile> profiles)
        {
            Mapper.Initialize(a =>
            {
                foreach (var profile in profiles)
                {
                    a.AddProfile(profile);
                }
            });
        }
    }
}
