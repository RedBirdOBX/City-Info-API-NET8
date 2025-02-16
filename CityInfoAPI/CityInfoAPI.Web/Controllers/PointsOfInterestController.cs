using CityInfoAPI.Data;
using CityInfoAPI.Dtos.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;

namespace CityInfoAPI.Controllers
{
    [Route("api/cities/{cityGuid}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        /// <summary>
        /// Gets all Points of Interest for City
        /// </summary>
        /// <returns></returns>
        [HttpGet("", Name = "GetPointsOfInterestForCity")]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterestForCity([FromRoute] Guid cityGuid)
        {
            var cities = CityInfoMemoryDataStore.Current.Cities;

            var city = cities.Where(c => c.CityGuid == cityGuid).FirstOrDefault();
            if (city == null)
            {
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cityGuid"></param>
        /// <param name="pointOfInterestGuid"></param>
        /// <returns></returns>
        [HttpGet("{pointOfInterestGuid}", Name = "GetPointOfInterestById")]
        public ActionResult<PointOfInterestDto> GetPointOfInterestById([FromRoute] Guid cityGuid, [FromRoute] Guid pointOfInterestGuid)
        {
            var cities = CityInfoMemoryDataStore.Current.Cities;

            var city = cities.Where(c => c.CityGuid == cityGuid).FirstOrDefault();
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterest = city.PointsOfInterest.Where(p => p.PointGuid == pointOfInterestGuid).FirstOrDefault();
            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(pointOfInterest);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cityGuid"></param>
        /// <param name="createPointOfInterest"></param>
        /// <returns></returns>
        [HttpPost("", Name = "CreatePointOfInterest")]
        public ActionResult CreatePointOfInterest([FromRoute] Guid cityGuid, [FromBody] PointOfInterestCreateDto createPointOfInterest)
        {
            var city = CityInfoMemoryDataStore.Current.Cities.Where(c => c.CityGuid == cityGuid).FirstOrDefault();
            if (city == null)
            {
                return NotFound();
            }

            // temp
            int max = CityInfoMemoryDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id) + 1;

            var finalPointOfInterest = new PointOfInterestDto
            {
                Id = max,
                PointGuid = createPointOfInterest.PointGuid,
                CityId = createPointOfInterest.CityId,
                CityGuid = cityGuid,
                Name = createPointOfInterest.Name,
                Description = createPointOfInterest.Description,
                CreatedOn = createPointOfInterest.CreatedOn.Date
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterestById", new { cityGuid = cityGuid, pointOfInterestGuid = finalPointOfInterest.PointGuid }, finalPointOfInterest);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cityGuid"></param>
        /// <param name="pointOfInterestGuid"></param>
        /// <param name="updatePointOfInterest"></param>
        /// <returns></returns>
        [HttpPut("{pointOfInterestGuid}", Name = "UpdatePointOfInterest")]
        public ActionResult<PointOfInterestDto> UpdatePointOfInterest([FromRoute] Guid cityGuid, [FromRoute] Guid pointOfInterestGuid, [FromBody] PointOfInterestUpdateDto updatePointOfInterest)
        {
            var city = CityInfoMemoryDataStore.Current.Cities.Where(c => c.CityGuid == cityGuid).FirstOrDefault();
            if (city == null)
            {
                return NotFound();
            }

            // find the point of interest
            var existingPointOfInterest = city.PointsOfInterest.Where(p => p.PointGuid == pointOfInterestGuid).FirstOrDefault();
            if (existingPointOfInterest == null)
            {
                return NotFound();
            }

            // does the point of interest belong to the city?
            if (existingPointOfInterest.CityGuid != cityGuid)
            {
                return NotFound();
            }

            existingPointOfInterest.Name = updatePointOfInterest.Name;
            existingPointOfInterest.Description = updatePointOfInterest.Description;

            return NoContent();
        }

        [HttpPatch("{pointOfInterestGuid}", Name = "PatchPointOfInterest")]
        public ActionResult<PointOfInterestDto> PatchPointOfInterest([FromRoute] Guid cityGuid, [FromRoute] Guid pointOfInterestGuid,
            [FromBody] JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
        {
            var city = CityInfoMemoryDataStore.Current.Cities.Where(c => c.CityGuid == cityGuid).FirstOrDefault();
            if (city == null)
            {
                return NotFound();
            }

            // find the point of interest
            var existingPointOfInterest = city.PointsOfInterest.Where(p => p.PointGuid == pointOfInterestGuid).FirstOrDefault();
            if (existingPointOfInterest == null)
            {
                return NotFound();
            }

            // does the point of interest belong to the city?
            if (existingPointOfInterest.CityGuid != cityGuid)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = new PointOfInterestUpdateDto
            {
                Name = existingPointOfInterest.Name,
                Description = existingPointOfInterest.Description
            };

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            // validate the final version
            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            existingPointOfInterest.Name = pointOfInterestToPatch.Name;
            existingPointOfInterest.Description = pointOfInterestToPatch.Description;

            return NoContent();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cityGuid"></param>
        /// <param name="pointOfInterestGuid"></param>
        /// <returns></returns>
        [HttpDelete("{pointOfInterestGuid}", Name = "DeletePointOfInterest")]
        public ActionResult DeletePointOfInterest([FromRoute] Guid cityGuid, [FromRoute] Guid pointOfInterestGuid)
        {
            var existingCity = CityInfoMemoryDataStore.Current.Cities.Where(c => c.CityGuid == cityGuid).FirstOrDefault();
            if (existingCity == null)
            {
                return NotFound();
            }

            // find the point of interest
            var existingPointOfInterest = existingCity.PointsOfInterest.Where(p => p.PointGuid == pointOfInterestGuid).FirstOrDefault();
            if (existingPointOfInterest == null)
            {
                return NotFound();
            }

            // does the point of interest belong to the city?
            if (existingPointOfInterest.CityGuid != cityGuid)
            {
                return NotFound();
            }

            existingCity.PointsOfInterest.Remove(existingPointOfInterest);

            return NoContent();
        }
    }
}
