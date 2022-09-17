using DemoApp.Entities;

namespace DemoApp.Services
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<(IEnumerable<City>, PaginationMetaData)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize);
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
        Task<City?> GetCityAsync(int cityId, bool includePoi);        
        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int poiId);
        Task<bool> CityExistsAsync(int cityId);        
        Task<bool> SaveChangesAsync();
        Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest poi);        
        void DeletePointOfInterest(PointOfInterest poi);
        Task<bool> CityNameMatchesCityId(string? cityName, int cityId);        
    }
}
