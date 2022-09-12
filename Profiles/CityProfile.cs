using AutoMapper;

namespace DemoApp.Profiles
{
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            CreateMap<Entities.City, Models.CitiesWithoutPointsOfInterestDTO>();
            CreateMap<Entities.City, Models.CitiesDTO>();            
        }
    }
}
