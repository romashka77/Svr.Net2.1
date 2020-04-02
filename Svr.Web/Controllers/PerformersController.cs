using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Extensions;
using Svr.Web.Extensions;
using Svr.Web.Models;
using Svr.Web.Models.PerformersViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    [AuthorizeRoles(Role.AdminOPFR, Role.UserOPFR, Role.AdminUPFR, Role.UserUPFR, Role.Administrator)]
    public class PerformersController : Controller
    {
        private readonly IPerformerRepository repository;
        private readonly IRegionRepository regionRepository;
        private readonly IDistrictRepository districtRepository;
        private readonly IDistrictPerformerRepository districtPerformerRepository;
        private readonly ILogger<PerformersController> logger;

        [TempData]
        public string StatusMessage { get; set; }
        #region Конструктор
        public PerformersController(IPerformerRepository repository, IRegionRepository regionRepository, IDistrictRepository districtRepository, IDistrictPerformerRepository districtPerformerRepository, ILogger<PerformersController> logger)
        {
            this.logger = logger;
            this.repository = repository;
            this.regionRepository = regionRepository;
            this.districtRepository = districtRepository;
            this.districtPerformerRepository = districtPerformerRepository;
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
        #region Index
        // GET: Performers
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string owner = null, string searchString = null, int page = 1, int itemsPage = 10)
        {
            var filterSpecification = new PerformerSpecification(owner.ToLong());
            var list = repository.List(filterSpecification);
            //фильтрация
            if (!String.IsNullOrEmpty(searchString))
            {
                list = list.Where(d => d.Name.ToUpper().Contains(searchString.ToUpper()));
            }
            // сортировка
            list = repository.Sort(list, sortOrder);
            // пагинация
            var count = await list.CountAsync();
            var itemsOnPage = await list.Skip((page - 1) * itemsPage).Take(itemsPage).AsNoTracking().ToListAsync();
            var indexModel = new IndexViewModel()
            {
                ItemViewModels = itemsOnPage.Select(i => new ItemViewModel()
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    CreatedOnUtc = i.CreatedOnUtc,
                    UpdatedOnUtc = i.UpdatedOnUtc,
                    Region = i.Region
                }),
                PageViewModel = new PageViewModel(count, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString, owner, (await regionRepository.ListAllAsync()).Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) })),
                StatusMessage = StatusMessage
            };
            return View(indexModel);
        }
        #endregion
        #region Details
        // GET: Performers/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, RegionId = item.RegionId, Region = item.Region, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, DistrictPerformers = item.DistrictPerformers };
            return View(model);
        }
        #endregion
        #region Create
        // GET: Performers/Create
        [AuthorizeRoles(Role.AdminOPFR, Role.AdminUPFR, Role.Administrator)]
        public async Task<IActionResult> Create()
        {
            ViewBag.Regions = new SelectList(await regionRepository.ListAllAsync(), "Id", "Name", 1);
            return View();
        }
        // POST: Performers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Role.AdminOPFR, Role.AdminUPFR, Role.Administrator)]
        public async Task<IActionResult> Create(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var item = await repository.AddAsync(new Performer { Name = model.Name, Description = model.Description, RegionId = model.RegionId });
                if (item != null)
                {
                    StatusMessage = item.MessageAddOk();
                    logger.LogInformation($"{model} create");
                    return RedirectToAction(nameof(Index));
                }
            }
            ModelState.AddModelError(string.Empty, model.MessageAddError());
            ViewBag.Regions = new SelectList(await regionRepository.ListAllAsync(), "Id", "Name", 1);
            return View(model);
        }
        #endregion
        #region Edit
        // GET: Performers/Edit/5
        [AuthorizeRoles(Role.AdminOPFR, Role.AdminUPFR, Role.Administrator)]
        public async Task<ActionResult> Edit(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, RegionId = item.RegionId, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, DistrictPerformers = item.DistrictPerformers };
            ViewBag.Regions = new SelectList(await regionRepository.ListAllAsync(), "Id", "Name", 1);
            ViewBag.Districts = await districtRepository.ListAsync(new DistrictSpecification(model.RegionId));
            return View(model);
        }
        // POST: Performers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Role.AdminOPFR, Role.AdminUPFR, Role.Administrator)]
        public async Task<IActionResult> Edit(ItemViewModel model, long[] selectedDistricts)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await districtPerformerRepository.ClearAsync(new PerformerDistrictSpecification(model.Id));
                    if (selectedDistricts != null)
                    {
                        foreach (var d in selectedDistricts)
                        {
                            await districtPerformerRepository.AddAsync(new DistrictPerformer { DistrictId = d, PerformerId = model.Id });
                        }
                    }
                    await repository.UpdateAsync(new Performer { Id = model.Id, Description = model.Description, Name = model.Name, CreatedOnUtc = model.CreatedOnUtc, RegionId = model.RegionId });
                    StatusMessage = model.MessageEditOk();
                    logger.LogInformation($"{model} edit");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!(await repository.EntityExistsAsync(model.Id)))
                    {
                        StatusMessage = $"{model.MessageEditError()} {ex.Message}";
                    }
                    else
                    {
                        StatusMessage = $"{model.MessageEditErrorNoknow()} {ex.Message}";
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Regions = new SelectList(await regionRepository.ListAllAsync(), "Id", "Name", 1);
            ViewBag.Districts = await districtRepository.ListAsync(new DistrictSpecification(model.RegionId));
            return View(model);
        }
        #endregion
        #region Delete
        // GET: Performers/Delete/5
        [AuthorizeRoles(Role.Administrator)]
        public async Task<IActionResult> Delete(long? id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, StatusMessage = StatusMessage };
            return View(model);
        }
        // POST: Performers/Delete/5
        [HttpPost, ActionName("Delete")]
        [AuthorizeRoles(Role.Administrator)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await repository.DeleteAsync(new Performer { Id = model.Id, Name = model.Name });
                StatusMessage = model.MessageDeleteOk();
                logger.LogInformation($"{model} delete");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                StatusMessage = $"{model.MessageDeleteError()} {ex.Message}.";
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion
    }
}
