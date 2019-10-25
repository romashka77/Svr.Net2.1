using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/Region")]
    [Authorize(Roles = "Администратор, Администратор ОПФР")]
    public class RegionController : Controller
    {
        private ILogger<RegionController> logger;
        private readonly IRegionRepository regionRepository;

        //private readonly DataContext _context;

        public RegionController(IRegionRepository regionRepository, ILogger<RegionController> logger = null)
        {
            this.logger = logger;
            this.regionRepository = regionRepository;
        }

        // GET: api/Region
        [HttpGet]
        public IEnumerable<Region> GetRegions()
        {
            return regionRepository.ListAll();
        }

        // GET: api/Region/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRegion([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var region = await regionRepository.GetByIdAsync(id);// .Regions.SingleOrDefaultAsync(m => m.Id == id);

            if (region == null)
            {
                return NotFound();
            }

            return Ok(region);
        }

        // PUT: api/Region/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRegion([FromRoute] long id, [FromBody] Region region)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != region.Id)
            {
                return BadRequest();
            }
            try
            {
                await regionRepository.UpdateAsync(region);
                logger.LogInformation($"{region} edite");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await RegionExists(id)))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // POST: api/Region
        [HttpPost]
        public async Task<IActionResult> PostRegion([FromBody] Region region)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await regionRepository.AddAsync(region);
            logger.LogInformation($"{region} create");
            return CreatedAtAction("GetRegion", new { id = region.Id }, region);
        }

        // DELETE: api/Region/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegion([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var region = await regionRepository.GetByIdAsync(id);
            if (region == null)
            {
                return NotFound();
            }
            await regionRepository.DeleteAsync(region);
            logger.LogInformation($"{region} delete");
            return Ok(region);
        }

        private async Task<bool> RegionExists(long id)
        {
            return await regionRepository.EntityExistsAsync(id);
        }
    }
}