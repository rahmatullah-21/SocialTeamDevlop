#region

using AutoMapper;

#endregion

namespace DominatorHouseCore.Utility
{
    public interface IMapperProvider
    {
        IMapper Mapper { get; }
    }

    public class MapperProvider : IMapperProvider
    {
        public IMapper Mapper { get; }

        public MapperProvider(Profile mapperProfiles)
        {
            var config = new MapperConfiguration(cfg =>
                cfg.AddProfile(mapperProfiles)
            );
            Mapper = config.CreateMapper();
        }
    }
}