using AutoMapper;
using DemoApp.Models;
using DemoApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DemoApp.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        const int maxCitiesPageSize = 20;

        public CitiesController(ICityInfoRepository cityInfoRepository,
            IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CitiesWithoutPointsOfInterestDTO>>> 
            GetCities(
                        [FromQuery] string? name, 
                        [FromQuery] string? searchQuery,
                        [FromQuery] int pageNumber = 1, 
                        [FromQuery] int pageSize = 10
            )
        {
            if (pageSize > maxCitiesPageSize)
            {
                pageSize = maxCitiesPageSize;
            }

            var cityEntities = await _cityInfoRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);
                        
            /*
            var results = new List<CitiesWithoutPointsOfInterestDTO>();
            foreach (var cityEntity in cityEntities)
            {
                results.Add(new CitiesWithoutPointsOfInterestDTO
                {
                    Id = cityEntity.Id,
                    Name = cityEntity.Name,
                    Description = cityEntity.Description
                });
            }
            */

            return Ok(_mapper.Map<IEnumerable<CitiesWithoutPointsOfInterestDTO>>(cityEntities));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id, bool includePoi)
        {
            var city = await _cityInfoRepository.GetCityAsync(id, includePoi);
            
            if (city == null)
            {
                return NotFound();
            }            
            else if (includePoi)
            {
                return Ok(_mapper.Map<CitiesDTO>(city));
            }
            else
            {
                return Ok(_mapper.Map<CitiesWithoutPointsOfInterestDTO>(city));
            }            
        }
    }
}
