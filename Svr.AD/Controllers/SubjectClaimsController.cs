using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Extensions;
using Svr.AD.Extensions;
using Svr.AD.Models;
using Svr.AD.Models.SubjectClaimsViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.AD.Controllers
{
    [AuthorizeRoles(Role.Admin)]
    public class SubjectClaimsController : Controller
    {
        private readonly IGroupClaimRepository groupClaimRepository;
        private readonly ISubjectClaimRepository repository;
        private readonly ILogger<SubjectClaimsController> logger;
        [TempData]
        public string StatusMessage { get; set; }
        #region Конструктор
        public SubjectClaimsController(IGroupClaimRepository groupClaimRepository, ISubjectClaimRepository repository, ILogger<SubjectClaimsController> logger)
        {
            this.logger = logger;
            this.groupClaimRepository = groupClaimRepository;
            this.repository = repository;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //groupClaimRepository = null;
                //subjectClaimRepository = null;
                //logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Index
        // GET: SubjectClaims
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string owner = null, string searchString = null, int page = 1, int itemsPage = 10)
        {
            var list = repository.List(new SubjectClaimSpecification(owner.ToLong()));
            //фильтрация
            if (!String.IsNullOrEmpty(searchString))
            {
                list = list.Where(d => d.Name.ToUpper().Contains(searchString.ToUpper()) || d.Code.ToUpper().Contains(searchString.ToUpper()));
            }
            // сортировка
            list = repository.Sort(list, sortOrder);
            // пагинация
            var count = await list.CountAsync();
            var itemsOnPage = await list.Skip((page - 1) * itemsPage).Take(itemsPage).ToListAsync();
            var indexModel = new IndexViewModel()
            {
                ItemViewModels = itemsOnPage.Select(i => new ItemViewModel()
                {
                    Id = i.Id,
                    Code = i.Code,
                    Name = i.Name,
                    Description = i.Description,
                    CreatedOnUtc = i.CreatedOnUtc,
                    UpdatedOnUtc = i.UpdatedOnUtc,
                    GroupClaim = i.GroupClaim
                }),
                PageViewModel = new PageViewModel(count, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString, owner, (await groupClaimRepository.ListAllAsync()).ToList().Select(a => new SelectListItem { Text = $"{a.Code} {a.Name}", Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) })),
                StatusMessage = StatusMessage
            };
            return View(indexModel);
        }
        #endregion
        #region Details
        // GET: SubjectClaims/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
                //throw new ApplicationException($"Не удалось загрузить район с ID {id}.");
            }
            var model = new ItemViewModel { Id = item.Id, Code = item.Code, Name = item.Name, Description = item.Description, GroupClaimId = item.GroupClaimId, GroupClaim = item.GroupClaim, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc };
            return View(model);
        }
        #endregion
        #region Create
        // GET: SubjectClaims/Create
        [AuthorizeRoles(Role.Admin)]
        public async Task<IActionResult> Create()
        {
            ViewBag.groupClaims = new SelectList((await groupClaimRepository.ListAllAsync()).Select(a => new { a.Id, Name = $"{a.Code} {a.Name}" }), "Id", "Name", 1);
            return View();
        }
        // POST: SubjectClaims/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Role.Admin)]
        public async Task<IActionResult> Create(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var item = await repository.AddAsync(new SubjectClaim { Code = model.Code, Name = model.Name, Description = model.Description, GroupClaimId = model.GroupClaimId });
                if (item != null)
                {
                    StatusMessage = item.MessageAddOk();
                    logger.LogInformation($"{model} create");
                    return RedirectToAction(nameof(Index));
                }
            }
            ModelState.AddModelError(string.Empty, model.MessageAddError());
            ViewBag.groupClaims = new SelectList((await groupClaimRepository.ListAllAsync()).Select(a => new { a.Id, Name = $"{a.Code} {a.Name}" }), "Id", "Name", 1);
            return View(model);
        }
        #endregion
        #region Edit
        // GET: SubjectClaims/Edit/5
        [AuthorizeRoles(Role.Admin)]
        public async Task<IActionResult> Edit(long? id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Code = item.Code, Name = item.Name, Description = item.Description, GroupClaimId = item.GroupClaimId, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc };
            ViewBag.groupClaims = new SelectList((await groupClaimRepository.ListAllAsync()).Select(a => new { a.Id, Name = $"{a.Code} {a.Name}" }), "Id", "Name", 1);
            return View(model);
        }
        // POST: SubjectClaims/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Role.Admin)]
        public async Task<IActionResult> Edit(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await repository.UpdateAsync(new SubjectClaim { Id = model.Id, Code = model.Code, Description = model.Description, Name = model.Name, CreatedOnUtc = model.CreatedOnUtc, GroupClaimId = model.GroupClaimId });
                    StatusMessage = model.MessageEditOk();
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
            ViewBag.groupClaims = new SelectList((await groupClaimRepository.ListAllAsync()).Select(a => new { a.Id, Name = $"{a.Code} {a.Name}" }), "Id", "Name", 1);
            logger.LogInformation($"{model} edit");
            return View(model);
        }
        #endregion
        #region Delete
        // GET: SubjectClaims/Delete/5
        [AuthorizeRoles(Role.Admin)]
        public async Task<IActionResult> Delete(long? id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Code = item.Code, Name = item.Name, Description = item.Description, GroupClaimId = item.GroupClaimId, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, StatusMessage = StatusMessage };
            return View(model);
        }

        // POST: SubjectClaims/Delete/5
        [HttpPost, ActionName("Delete")]
        [AuthorizeRoles(Role.Admin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await repository.DeleteAsync(new SubjectClaim { Id = model.Id, Name = model.Name, Code = model.Code, });
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
