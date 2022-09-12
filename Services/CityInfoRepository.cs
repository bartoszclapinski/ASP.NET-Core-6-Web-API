﻿using DemoApp.DbContexts;
using DemoApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoApp.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePoi)
        {
            if (includePoi)
            {
                return await _context.Cities
                    .Include(c => c.PointsOfInterest)
                    .Where(c => c.Id == cityId)
                    .FirstOrDefaultAsync();
            }
            else
            {
                return await _context.Cities
                    .Where(c => c.Id == cityId)
                    .FirstOrDefaultAsync();
            }
            
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int poiId)
        {
            return await _context.PointsOfInterest
                .Where(p => p.CityId == cityId && p.Id == poiId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
        {
            return await _context.PointsOfInterest
                .Where(p => p.CityId == cityId)
                .ToListAsync();
        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest poi)
        {
            var city = await GetCityAsync(cityId, false);
            if (city != null)
            {
                city.PointsOfInterest.Add(poi); 
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);    
        }

        public void DeletePointOfInterest(PointOfInterest poi)
        {
            _context.PointsOfInterest.Remove(poi);
        }
    }
}