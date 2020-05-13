using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Infrastructure.Extensions;
using Svr.Infrastructure.Identity;
using Svr.Web.Extensions;
using Svr.Web.Models;
using Svr.Web.Models.RegionsViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    [AuthorizeRoles(Role.AdminOPFR, Role.UserOPFR, Role.AdminUPFR, Role.UserUPFR, Role.Administrator)]
    public class RegionsController : MessageController
    {
        private readonly ILogger<RegionsController> logger;
        private readonly IRegionRepository repository;
        private readonly UserManager<ApplicationUser> userManager;
        #region Конструктор
        public RegionsController(IRegionRepository repository, ILogger<RegionsController> logger, UserManager<ApplicationUser> userManager)
        {
            //this.regionService = regionService;
            this.logger = logger;
            this.repository = repository;
            this.userManager = userManager;
        }
        #endregion
        //public async Task<ActionResult> GetRegionsDropdown(int selectedId)
        //{
        //    if (User.IsInRole("Администратор"))
        //    {

        //        ViewData.Model = (await repository.ListAllAsync()).Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (selectedId == a.Id) }).OrderBy(a => a.Text);
        //    }
        //    else
        //    {
        //        ViewData.Model = (await repository.ListAllAsync()).Where(n => n.Id == selectedId).Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (selectedId == a.Id) }).OrderBy(a => a.Text);
        //    }
        //    return View("Dropdown");
        //}


        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //repository = null;
                //logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Index
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string searchString = null, string lord = null, int page = 1, int itemsPage = 10)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            if (!User.IsInRole(Role.Administrator))
            {
                lord = user?.RegionId.ToString();
            }
            //фильтрация
            var list = repository.Filter(searchString: searchString, lord: lord, flgFilter: !User.IsInRole(Role.Administrator));
            //сортировка
            list = repository.Sort(list, sortOrder);
            //пагинация
            var totalItems = await list.CountAsync();
            var itemsOnPage = await list.Skip((page - 1) * itemsPage).Take(itemsPage).AsNoTracking().ToListAsync();
            var indexModel = new IndexViewModel()
            {
                ItemViewModels = itemsOnPage.Select(i => new ItemViewModel()
                {
                    Id = i.Id,
                    Code = i.Code,
                    Name = i.Name,
                    Description = i.Description,
                    //Districts=i.Districts,
                    CreatedOnUtc = i.CreatedOnUtc,
                    UpdatedOnUtc = i.UpdatedOnUtc
                }),
                PageViewModel = new PageViewModel(totalItems, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString: searchString, itemsCount: totalItems),
                StatusMessage = StatusMessage
            };
            return View(indexModel);
        }
        #endregion
        #region Details
        // GET: Regions/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var region = await repository.GetByIdWithItemsAsync(id);
            if (region == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = region.Id, Code = region.Code, Name = region.Name, Description = region.Description, Districts = region.Districts, StatusMessage = StatusMessage, CreatedOnUtc = region.CreatedOnUtc, UpdatedOnUtc = region.UpdatedOnUtc, Performers = region.Performers };
            return View(model);
        }
        #endregion
        #region Create
        // GET: Regions/Create
        [AuthorizeRoles(Role.Administrator)]
        public IActionResult Create()
        {
            return View();
        }
        // POST: Regions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Role.Administrator)]
        public async Task<IActionResult> Create(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                //добавляем новый регион
                var item = await repository.AddAsync(new Region { Code = model.Code, Name = model.Name, Description = model.Description });
                if (item != null)
                {
                    StatusMessage = item.MessageAddOk();
                    logger.LogInformation($"{model} create");
                    return RedirectToAction(nameof(Index));
                }
            }
            ModelState.AddModelError(string.Empty, model.MessageAddError());
            return View(model);
        }
        #endregion
        #region Edit
        // GET: Regions/Edit/5
        [AuthorizeRoles(Role.Administrator)]
        public async Task<IActionResult> Edit(long? id)
        {
            var region = await repository.GetByIdAsync(id);
            if (region == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = region.Id, Code = region.Code, Name = region.Name, Description = region.Description, StatusMessage = StatusMessage, CreatedOnUtc = region.CreatedOnUtc };
            return View(model);
        }

        // POST: Regions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Role.Administrator)]
        public async Task<IActionResult> Edit(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await repository.UpdateAsync(new Region { Id = model.Id, Code = model.Code, Description = model.Description, Name = model.Name, Districts = model.Districts, CreatedOnUtc = model.CreatedOnUtc });
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
            return View(model);
        }
        #endregion
        #region Delete
        // GET: Regions/Delete/5
        [AuthorizeRoles(Role.Administrator)]
        public async Task<IActionResult> Delete(long? id)
        {
            var region = await repository.GetByIdAsync(id);
            if (region == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = region.Id, Code = region.Code, Name = region.Name, Description = region.Description };
            return View(model);
        }

        // POST: Regions/Delete/5
        [HttpPost, ActionName("Delete")]
        [AuthorizeRoles(Role.Administrator)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await repository.DeleteAsync(new Region { Id = model.Id, Name = model.Name, Code = model.Code });
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
