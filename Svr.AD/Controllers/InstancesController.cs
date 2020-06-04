using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Extensions;
using Svr.AD.Models;
using Svr.AD.Models.InstanceViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Svr.Utils.Roles;
using Svr.Utils.Controllers;

namespace Svr.AD.Controllers
{
    [AuthorizeRoles(Role.Admin, Role.Users, Role.Manager)]
    public class InstancesController : MessageController
    {
        private readonly IInstanceRepository repository;
        private readonly IClaimRepository сlaimRepository;
        private readonly IDirRepository dirRepository;
        private readonly ILogger<InstancesController> logger;
        //private readonly UserManager<ApplicationUser> userManager;
        #region Конструктор
        public InstancesController(IInstanceRepository repository, IClaimRepository сlaimRepository, IDirRepository dirRepository, ILogger<InstancesController> logger)
        {
            this.logger = logger;
            this.repository = repository;
            this.сlaimRepository = сlaimRepository;
            this.dirRepository = dirRepository;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //сlaimRepository = null;
                //repository = null;
                //dirRepository = null;
                //logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Index
        // GET: Instances
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string owner = null, string searchString = null, int page = 1, int itemsPage = 10, DateTime? dateS = null, DateTime? datePo = null)
        {
            if (string.IsNullOrWhiteSpace(owner))
            {
                throw new ArgumentNullException(nameof(owner));
                //var user = await userManager.FindByNameAsync(User.Identity.Name);
                //if (user != null)
                //{
                //    owner = user.DistrictId.ToString();
                //}
            }
            var list = repository.List(new InstanceSpecification(owner.ToLong()));

            //фильтрация
            if (!String.IsNullOrEmpty(searchString))
            {
                list = list.Where(d => d.Name.ToUpper().Contains(searchString.ToUpper()));
            }
            if (dateS != null)
                list = list.Where(c => c.DateInCourtDecision >= dateS);
            if (datePo != null)
                list = list.Where(c => c.DateInCourtDecision <= datePo);

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
                    DateInCourtDecision = i.DateInCourtDecision,
                    Claim = i.Claim,
                    Number = i.Number
                }),
                Claim = (await сlaimRepository.GetByIdAsync(owner.ToLong())),
                PageViewModel = new PageViewModel(count, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString, owner),
                StatusMessage = StatusMessage
            };
            return View(indexModel);
        }
        #endregion
        #region Details
        // GET: Instances/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                //return RedirectToAction(nameof(Index), new { owner = model.ClaimId });
                throw new ApplicationException(id.ToString().ErrorFind());
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, Claim = item.Claim, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, CourtDecision = item.CourtDecision, DateCourtDecision = item.DateCourtDecision, DateInCourtDecision = item.DateInCourtDecision, DateTransfer = item.DateTransfer, DutyDenied = item.DutyDenied, DutyPaid = item.DutyPaid, DutySatisfied = item.DutySatisfied, ServicesDenied = item.ServicesDenied, ServicesSatisfied = item.ServicesSatisfied, SumDenied = item.SumDenied, SumSatisfied = item.SumSatisfied, СostDenied = item.СostDenied, СostSatisfied = item.СostSatisfied, ClaimId = item.ClaimId, CourtDecisionId = item.CourtDecisionId, Number = item.Number };
            return View(model);
        }
        #endregion
        #region Create
        // GET: Instances/Create
        [AuthorizeRoles(Role.Admin, Role.Users)]
        public async Task<IActionResult> Create(long owner)
        {
            ViewBag.Number = (await repository.ListAsync(new InstanceSpecification(owner))).Count() + 1;
            ViewBag.Owner = owner;
            switch (ViewBag.Number)
            {
                case 1:
                    ViewBag.CourtDecisions = new SelectList(await dirRepository.ListAsync(new DirSpecification("Решения суда 1-ой инстанции")), "Id", "Name", null);
                    break;
                case 2:
                    ViewBag.CourtDecisions = new SelectList(await dirRepository.ListAsync(new DirSpecification("Решения суда 2-ой инстанции")), "Id", "Name", null);
                    break;
                case 3:
                    ViewBag.CourtDecisions = new SelectList(await dirRepository.ListAsync(new DirSpecification("Решения суда 3-ей инстанции")), "Id", "Name", null);
                    break;
                case 4:
                    ViewBag.CourtDecisions = new SelectList(await dirRepository.ListAsync(new DirSpecification("Решения суда 4-ой инстанции")), "Id", "Name", null);
                    break;
                default:
                    ViewBag.CourtDecisions = null;
                    break;
            }

            return View();
        }

        // POST: Instances/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Role.Admin, Role.Users)]
        public async Task<IActionResult> Create(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var item = await repository.AddAsync(new Instance { Name = model.Name, ClaimId = model.ClaimId, Description = model.Description, Claim = model.Claim, CourtDecision = model.CourtDecision, DateCourtDecision = model.DateCourtDecision, СostSatisfied = model.СostSatisfied, СostDenied = model.СostDenied, DateInCourtDecision = model.DateInCourtDecision, DateTransfer = model.DateTransfer, DutyDenied = model.DutyDenied, DutyPaid = model.DutyPaid, DutySatisfied = model.DutySatisfied, ServicesDenied = model.ServicesDenied, ServicesSatisfied = model.ServicesSatisfied, SumDenied = model.SumDenied, SumSatisfied = model.SumSatisfied, CourtDecisionId = model.CourtDecisionId, Number = model.Number });
                if (item != null)
                {
                    StatusMessage = item.MessageAddOk();
                    logger.LogInformation($"{model} create");
                    return RedirectToAction(nameof(Index), new { owner = item.ClaimId });
                }
            }
            ModelState.AddModelError(string.Empty, model.MessageAddError());
            await SetViewBag(model);

            return View(model);
        }
        #endregion
        #region Edit
        // GET: Instances/Edit/5
        [AuthorizeRoles(Role.Admin, Role.Users)]
        public async Task<ActionResult> Edit(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                //return RedirectToAction(nameof(Index));
                throw new ApplicationException(id.ToString().ErrorFind());
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, СostDenied = item.СostDenied, СostSatisfied = item.СostSatisfied, SumSatisfied = item.SumSatisfied, SumDenied = item.SumDenied, ServicesSatisfied = item.ServicesSatisfied, ServicesDenied = item.ServicesDenied, CourtDecision = item.CourtDecision, DateCourtDecision = item.DateCourtDecision, DateInCourtDecision = item.DateInCourtDecision, DateTransfer = item.DateTransfer, DutyDenied = item.DutyDenied, DutyPaid = item.DutyPaid, DutySatisfied = item.DutySatisfied, Claim = item.Claim, ClaimId = item.ClaimId, Number = item.Number, CourtDecisionId = item.CourtDecisionId };
            await SetViewBag(model);
            return View(model);
        }
        // POST: Instances/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Role.Admin, Role.Users)]
        public async Task<IActionResult> Edit(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await repository.UpdateAsync(new Instance { Id = model.Id, Description = model.Description, Name = model.Name, CreatedOnUtc = model.CreatedOnUtc, ClaimId = model.ClaimId, CourtDecision = model.CourtDecision, DateCourtDecision = model.DateCourtDecision, DateInCourtDecision = model.DateInCourtDecision, DateTransfer = model.DateTransfer, DutyDenied = model.DutyDenied, DutySatisfied = model.DutySatisfied, DutyPaid = model.DutyPaid, ServicesDenied = model.ServicesDenied, SumDenied = model.SumDenied, ServicesSatisfied = model.ServicesSatisfied, SumSatisfied = model.SumSatisfied, СostDenied = model.СostDenied, СostSatisfied = model.СostSatisfied, Number = model.Number, CourtDecisionId = model.CourtDecisionId });
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
            await SetViewBag(model);
            return View(model);
        }
        #endregion
        #region Delete
        // GET: Instances/Delete/5
        [AuthorizeRoles(Role.Admin)]
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

        // POST: Instances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Role.Admin)]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await repository.DeleteAsync(new Instance { Id = model.Id, Name = model.Name });
                StatusMessage = model.MessageDeleteOk();
                logger.LogInformation($"{model} delete");
                return RedirectToAction(nameof(Index), new { owner = model.ClaimId });
            }
            catch (Exception ex)
            {
                StatusMessage = $"{model.MessageDeleteError()} {ex.Message}.";
                logger.LogInformation($"{model.MessageDeleteError()} delete");
                return RedirectToAction(nameof(Index), new { owner = model.ClaimId });
            }
        }
        #endregion
        #region SetViewBag
        private async Task SetViewBag(ItemViewModel model)
        {
            switch (model.Number)
            {
                case 1:
                    ViewBag.CourtDecisions = new SelectList(await dirRepository.ListAsync(new DirSpecification("Решения суда 1-ой инстанции")), "Id", "Name", null);
                    break;
                case 2:
                    ViewBag.CourtDecisions = new SelectList(await dirRepository.ListAsync(new DirSpecification("Решения суда 2-ой инстанции")), "Id", "Name", null);
                    break;
                case 3:
                    ViewBag.CourtDecisions = new SelectList(await dirRepository.ListAsync(new DirSpecification("Решения суда 3-ей инстанции")), "Id", "Name", null);
                    break;
                case 4:
                    ViewBag.CourtDecisions = new SelectList(await dirRepository.ListAsync(new DirSpecification("Решения суда 4-ой инстанции")), "Id", "Name", null);
                    break;
                default:
                    ViewBag.CourtDecisions = null;
                    break;
            }
        }
        #endregion
    }
}
