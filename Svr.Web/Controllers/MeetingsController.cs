using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Extensions;
using Svr.Web.Extensions;
using Svr.Web.Models;
using Svr.Web.Models.MeetingsViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    [AuthorizeRoles(Role.AdminOPFR, Role.UserOPFR, Role.AdminUPFR, Role.UserUPFR, Role.Administrator)]
    public class MeetingsController : Controller
    {
        private readonly IMeetingRepository repository;
        private readonly IClaimRepository сlaimRepository;
        private readonly ILogger<MeetingsController> logger;

        [TempData]
        public string StatusMessage { get; set; }
        #region Конструктор
        public MeetingsController(IMeetingRepository repository, IClaimRepository сlaimRepository, ILogger<MeetingsController> logger)
        {
            this.logger = logger;
            this.repository = repository;
            this.сlaimRepository = сlaimRepository;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //сlaimRepository = null;
                //repository = null;
                //logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Index
        // GET: Meetings
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string owner = null, string searchString = null, int page = 1, int itemsPage = 10)
        {
            var list = repository.List(new MeetingSpecification(owner.ToLong()));
            //фильтрация
            if (!String.IsNullOrEmpty(searchString))
            {
                list = list.Where(d => d.Name.ToUpper().Contains(searchString.ToUpper()));
            }
            // сортировка

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
                    Claim = i.Claim,
                    Number = i.Number,
                    Date = i.Date,
                    Time = i.Time
                }),
                Claim = (await сlaimRepository.GetByIdAsync(owner.ToLong())),
                PageViewModel = new PageViewModel(count, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString: searchString, owner: owner, itemsCount: count),

                StatusMessage = StatusMessage
            };

            return View(indexModel);
        }
        #endregion
        #region Details
        // GET: Meetings/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                //return RedirectToAction(nameof(Index));
                //return RedirectToAction(nameof(Index), new { owner = model.ClaimId });
                throw new ApplicationException(id.ToString().ErrorFind());
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, Claim = item.Claim, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, ClaimId = item.ClaimId, Number = item.Number, Date = item.Date, Time = item.Time };
            return View(model);
        }
        #endregion
        #region Create
        // GET: Meetings/Create
        [[AuthorizeRoles(Role.AdminUPFR, Role.UserUPFR, Role.Administrator)]
        public IActionResult Create(long owner)
        {
            ViewBag.Owner = owner;
            return View();
        }

        // POST: Meetings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Role.AdminUPFR, Role.UserUPFR, Role.Administrator)]
        public async Task<IActionResult> Create(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                // добавляем новый Район
                var item = await repository.AddAsync(new Meeting { Name = model.Name, ClaimId = model.ClaimId, Description = model.Description, Claim = model.Claim, Number = model.Number, Date = model.Date, Time = model.Time, });
                if (item != null)
                {
                    StatusMessage = item.MessageAddOk();
                    logger.LogInformation($"{model} create");
                    return RedirectToAction(nameof(Index), new { owner = item.ClaimId });
                }
            }
            ModelState.AddModelError(string.Empty, model.MessageAddError());
            return View(model);
        }
        #endregion
        #region Edit
        // GET: Meetings/Edit/5
        [AuthorizeRoles(Role.AdminUPFR, Role.UserUPFR, Role.Administrator)]
        public async Task<ActionResult> Edit(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                //return RedirectToAction(nameof(Index));
                throw new ApplicationException(id.ToString().ErrorFind());
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, Claim = item.Claim, ClaimId = item.ClaimId, Number = item.Number, Date = item.Date, Time = item.Time };
            return View(model);
        }

        // POST: Meetings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Role.AdminUPFR, Role.UserUPFR, Role.Administrator)]
        public async Task<IActionResult> Edit(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await repository.UpdateAsync(new Meeting { Id = model.Id, Description = model.Description, Name = model.Name, CreatedOnUtc = model.CreatedOnUtc, ClaimId = model.ClaimId, Number = model.Number, Date = model.Date, Time = model.Time });
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
                //return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        #endregion
        #region Delete
        // GET: Meetings/Delete/5
        [AuthorizeRoles(Role.AdminUPFR, Role.UserUPFR, Role.Administrator)]
        public async Task<IActionResult> Delete(long? id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                //return RedirectToAction(nameof(Index));
                throw new ApplicationException(id.ToString().ErrorFind());
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, StatusMessage = StatusMessage, ClaimId = item.ClaimId };
            return View(model);
        }

        // POST: Meetings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Role.AdminUPFR, Role.UserUPFR, Role.Administrator)]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await repository.DeleteAsync(new Meeting { Id = model.Id, Name = model.Name });
                StatusMessage = model.MessageDeleteOk();
                logger.LogInformation($"{model} delete");
                return RedirectToAction(nameof(Index), new { owner = model.ClaimId });
            }
            catch (Exception ex)
            {
                StatusMessage = $"{model.MessageDeleteError()} {ex.Message}.";
                return RedirectToAction(nameof(Index), new { owner = model.ClaimId });
            }
        }
        #endregion
    }
}
