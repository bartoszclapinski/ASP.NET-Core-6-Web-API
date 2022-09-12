using AutoMapper;
using DemoApp.Models;
using DemoApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace DemoApp.Controllers
{    
    [ApiController]
    [Route("api/cities/{cityId}/pointsofinterest")]
    public class PointOfInterestController : ControllerBase
    {
        private readonly ILogger<PointOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointOfInterestController(
            ILogger<PointOfInterestController> logger,
            IMailService mailService, 
            ICityInfoRepository cityInfoRepository,
            IMapper mapper)        {

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDTO>>> GetPoi(int cityId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation(
                    $"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound();
            }
            else
            {
                var poiForCity = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);
                return Ok(_mapper.Map<IEnumerable<PointOfInterestDTO>>(poiForCity));
            }
            

        }

        [HttpGet("{poiId}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<IEnumerable<PointOfInterestDTO>>> GetPoi(int cityId, int poiId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }    
            else
            {
                var poi = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, poiId);
                
                if (poi == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(_mapper.Map<PointOfInterestDTO>(poi));
                }
            }
        }        
        
        [HttpPost]
        public async Task<ActionResult<PointOfInterestDTO>> CreatePointOfInterest (
            int cityId, PointOfInterestForCreationDTO pointOfInterest)
        {           

            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }
            else
            {
                var finalPoi = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

                await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPoi);

                await _cityInfoRepository.SaveChangesAsync();

                var createdPoi = _mapper.Map<Models.PointOfInterestDTO>(finalPoi);

                return CreatedAtAction(nameof(GetPoi),
                    new
                    {
                        cityId = cityId,
                        pointOfInterest = createdPoi.Id
                    },
                    createdPoi);
            }
        }


        [HttpPut("{poiId}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int poiId,
            PointOfInterestForUpdateDTO poi)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }
            else
            {
                var poiFromEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, poiId);

                if (poiFromEntity == null)
                {
                    return NotFound();
                }
                else
                {
                    _mapper.Map(poi, poiFromEntity);
                    await _cityInfoRepository.SaveChangesAsync();

                    return NoContent();
                }


            }
        }

        
        [HttpPatch("{poiId}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int poiId,
            JsonPatchDocument<PointOfInterestForUpdateDTO> patchDocument)
        {
            
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }            

            var poi = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, poiId);
            if (poi == null)
            {
                return NotFound();
            }

            var poiToPatch = _mapper.Map<PointOfInterestForUpdateDTO>(poi);

            patchDocument.ApplyTo(poiToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if (!TryValidateModel(poiToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(poiToPatch, poi);

            return NoContent();
            
        }

        
        [HttpDelete("{poiId}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int poiId)
        {
            if(!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var poi = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, poiId);
            if (poi == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInteres(poi);
            await _cityInfoRepository.SaveChangesAsync();

            _mailService.Send(
                "Point of Interest Deleted",
                $"Point of interest {poi.Name} with id {poi.Id} has been deleted.");

            return NoContent();
        }
        
    }
}
