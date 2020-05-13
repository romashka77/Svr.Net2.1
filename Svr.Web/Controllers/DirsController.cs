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
using Svr.Web.Models.DirViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    [AuthorizeRoles(Role.AdminOPFR, Role.UserOPFR, Role.AdminUPFR, Role.UserUPFR, Role.Administrator)]
    public class DirsController : Controller
    {
        private readonly IDirRepository repository;
        private readonly IDirNameRepository repositoryDirName;
        private readonly ILogger<DirsController> logger;

        [TempData]
        public string StatusMessage { get; set; }
        #region Конструктор
        public DirsController(IDirRepository repository, IDirNameRepository repositoryDirName, ILogger<DirsController> logger)
        {
            this.logger = logger;
            this.repository = repository;
            this.repositoryDirName = repositoryDirName;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //repository = null;
                //repositoryDirName = null;
                //logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Index
        // GET: Dirs
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string owner = null, string searchString = null, int page = 1, int itemsPage = 10)
        {
            //фильтрация
            var list = repository.List(new DirSpecification(owner.ToLong()));
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
                    CreatedOnUtc = i.CreatedOnUtc,
                    UpdatedOnUtc = i.UpdatedOnUtc,
                    DirName = i.DirName
                }),
                PageViewModel = new PageViewModel(count, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString, owner, (await repositoryDirName.ListAllAsync()).Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) })),
                StatusMessage = StatusMessage
            };
            return View(indexModel);
        }
        #endregion
        #region Details
        // GET: Dirs/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, DirNameId = item.DirNameId, DirName = item.DirName, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, Applicants = item.Applicants };
            return View(model);
        }
        #endregion
        #region Create
        // GET: Dirs/Create
        [AuthorizeRoles(Role.AdminOPFR, Role.Administrator)]
        public async Task<IActionResult> Create(int id)
        {
            ViewBag.DirNames = new SelectList(await repositoryDirName.ListAllAsync(), "Id", "Name", id);
            return View();
        }

        // POST: Dirs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Role.AdminOPFR, Role.Administrator)]
        public async Task<IActionResult> Create(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var item = await repository.AddAsync(new Dir { Name = model.Name, DirNameId = model.DirNameId });
                if (item != null)
                {
                    StatusMessage = item.MessageAddOk();
                    logger.LogInformation($"{model} create");
                    return RedirectToAction(nameof(Index));
                }
            }
            ModelState.AddModelError(string.Empty, model.MessageAddError());
            ViewBag.DirNames = new SelectList(await repositoryDirName.ListAllAsync(), "Id", "Name", model.DirNameId);
            return View(model);
        }
        #endregion
        #region Edit
        // GET: Dirs/Edit/5
        [AuthorizeRoles(Role.AdminOPFR, Role.Administrator)]
        public async Task<IActionResult> Edit(long? id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, DirNameId = item.DirNameId, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc };
            ViewBag.DirNames = new SelectList(await repositoryDirName.ListAllAsync(), "Id", "Name", model.DirNameId);
            return View(model);
        }
        // POST: Dirs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Role.AdminOPFR, Role.Administrator)]
        public async Task<IActionResult> Edit(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await repository.UpdateAsync(new Dir { Id = model.Id, Name = model.Name, CreatedOnUtc = model.CreatedOnUtc, DirNameId = model.DirNameId });
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
            ViewBag.DirNames = new SelectList(await repositoryDirName.ListAllAsync(), "Id", "Name", model.DirNameId);
            return View(model);
        }
        #endregion
        #region Delete
        // GET: Dirs/Delete/5
        [AuthorizeRoles(Role.Administrator)]
        public async Task<IActionResult> Delete(long? id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, DirNameId = item.DirNameId, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, StatusMessage = StatusMessage };
            return View(model);
        }

        // POST: Dirs/Delete/5
        [HttpPost, ActionName("Delete")]
        [AuthorizeRoles(Role.Administrator)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await repository.DeleteAsync(new Dir { Id = model.Id, Name = model.Name });
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
