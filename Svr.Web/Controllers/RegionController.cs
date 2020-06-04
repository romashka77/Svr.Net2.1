using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Utils.Roles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/Region")]
    [AuthorizeRoles(Role.AdminOPFR, Role.Administrator)]
    public class RegionController : Controller
    {
        private ILogger<RegionController> logger;
        private readonly IRegionRepository regionRepository;

        //private readonly DataContext _context;
        #region Конструктор
        public RegionController(IRegionRepository regionRepository, ILogger<RegionController> logger = null)
        {
            this.logger = logger;
            this.regionRepository = regionRepository;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //districtRepository = null;
                //repository = null;
                //regionRepository = null;
                //districtPerformerRepository = null;
                //logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion
        #region GetRegions
        // GET: api/Region
        [HttpGet]
        public IEnumerable<Region> GetRegions()
        {
            return regionRepository.ListAll();
        }
        #endregion
        #region GetRegion
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
        #endregion
        #region PutRegion
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
        #endregion
        #region PostRegion
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
        #endregion
        #region DeleteRegion
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
        #endregion
        #region RegionExists
        private async Task<bool> RegionExists(long id)
        {
            return await regionRepository.EntityExistsAsync(id);
        }
        #endregion
    }
}