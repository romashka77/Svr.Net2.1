﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Infrastructure.Extensions;
using Svr.Infrastructure.Identity;
using Svr.Utils.Controllers;
using Svr.Utils.Models;
using Svr.Utils.Roles;
using Svr.Utils.Models.ClaimsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    [AuthorizeRoles(Role.AdminOPFR, Role.UserOPFR, Role.AdminUPFR, Role.UserUPFR, Role.Administrator)]
    public class ClaimsController : MessageController
    {
        private readonly IClaimRepository repository;
        private readonly IDistrictRepository districtRepository;
        private readonly IRegionRepository regionRepository;
        private readonly ICategoryDisputeRepository categoryDisputeRepository;
        private readonly IGroupClaimRepository groupClaimRepository;
        private readonly ISubjectClaimRepository subjectClaimRepository;
        private readonly IDirRepository dirRepository;
        private readonly IApplicantRepository applicantRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<ClaimsController> logger;
        private readonly IInstanceRepository instanceRepository;
        #region Конструктор
        public ClaimsController(IClaimRepository repository, IRegionRepository regionRepository, IDistrictRepository districtRepository, ICategoryDisputeRepository categoryDisputeRepository, IGroupClaimRepository groupClaimRepository, ISubjectClaimRepository subjectClaimRepository, IDirRepository dirRepository, IApplicantRepository applicantRepository, ILogger<ClaimsController> logger, UserManager<ApplicationUser> userManager, IInstanceRepository instanceRepository)
        {
            this.logger = logger;
            this.repository = repository;
            this.regionRepository = regionRepository;
            this.districtRepository = districtRepository;
            this.categoryDisputeRepository = categoryDisputeRepository;
            this.groupClaimRepository = groupClaimRepository;
            this.subjectClaimRepository = subjectClaimRepository;
            this.dirRepository = dirRepository;
            this.applicantRepository = applicantRepository;
            this.userManager = userManager;
            this.instanceRepository = instanceRepository;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }
        #endregion
        #region Index
        // GET: Claims
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string lord = null, string owner = null, string searchString = null, int page = 1, int itemsPage = 5, DateTime? dateS = null, DateTime? datePo = null, string category = null, string groupClaim = null, string subjectClaim = null, string resultClaim = null)
        {
            if (User.IsInRole(Role.Administrator))
            { }
            else if (User.IsInRole(Role.AdminOPFR) || User.IsInRole(Role.UserOPFR))
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                lord = user?.RegionId.ToString();
            }
            else if (User.IsInRole(Role.AdminUPFR) || User.IsInRole(Role.UserUPFR))
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                lord = user?.RegionId.ToString();
                owner = user?.DistrictId.ToString();
            }
            //фильтрация
            var list = repository.Filter(searchString: searchString, owner: owner, dateS: dateS, datePo: datePo, category: category, groupClaim: groupClaim, subjectClaim: subjectClaim, resultClaim: resultClaim);
            // сортировка
            list = repository.Sort(list, sortOrder);
            // пагинация
            var itemsCount = await list.CountAsync();
            var itemsOnPage = await list.Skip((page - 1) * itemsPage).Take(itemsPage).AsNoTracking().ToListAsync();
            var indexModel = new IndexViewModel()
            {
                ItemViewModels = itemsOnPage.Select(i => new ItemViewModel()
                {
                    Id = i.Id,
                    Code = i.Code,
                    Name = i.Name,
                    NumPFR = i.NumPFR,
                    Description = i.Description,
                    //CreatedOnUtc = i.CreatedOnUtc,
                    //UpdatedOnUtc = i.UpdatedOnUtc,
                    DateIn = i.DateIn,
                    CategoryDispute = i.CategoryDispute,
                    GroupClaim = i.GroupClaim,
                    SubjectClaim = i.SubjectClaim,
                    District = i.District,
                    Sum = i.Sum,

                }),
                PageViewModel = new PageViewModel(itemsCount, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString: searchString, owner: owner, owners: await GetDistrictSelectList(lord, owner), lord: lord, lords: await GetRegionSelectList(lord), dateS: dateS, datePo: datePo, category: category, categores: await GetCategoreSelectList(category), groupClaim: groupClaim, groupClaims: await GetGroupClaimSelectList(category, groupClaim), subjectClaim: subjectClaim, subjectClaims: await GetSubjectClaimSelectList(groupClaim, subjectClaim), resultClaim: resultClaim, resultClaims: await GetResultClaimSelectList(), itemsCount: itemsCount),


                StatusMessage = StatusMessage
            };
            return View(indexModel);
        }
        #endregion
        #region GetSelectList
        private async Task<IEnumerable<SelectListItem>> GetResultClaimSelectList(string resultClaim = null)
        {
            return await dirRepository.Filter(lord: "Решения суда 1-ой инстанции").Select(a => new SelectListItem { Text = a.Name, Value = a.Name, Selected = (resultClaim == a.Name) }).OrderBy(a => a.Text).ToListAsync();
        }
        private async Task<IEnumerable<SelectListItem>> GetRegionSelectList(string lord)
        {
            return await regionRepository.Filter(lord: lord, flgFilter: !User.IsInRole(Role.Administrator)).Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (lord == a.Id.ToString()) }).OrderBy(a => a.Text).ToListAsync();
        }
        private async Task<IEnumerable<SelectListItem>> GetDistrictSelectList(string lord, string owner)
        {
            return await districtRepository.Filter(lord: lord, owner: owner, flgFilter: (User.IsInRole(Role.AdminUPFR) || User.IsInRole(Role.UserUPFR))).Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) }).OrderBy(a => a.Text).ToListAsync();
        }
        private async Task<IEnumerable<SelectListItem>> GetCategoreSelectList(string category)
        {
            return await categoryDisputeRepository.Filter().Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (category == a.Id.ToString()) }).OrderBy(a => a.Text).ToListAsync();
        }
        private async Task<IEnumerable<SelectListItem>> GetGroupClaimSelectList(string category, string groupClaim)
        {
            return await groupClaimRepository.Filter(category: category).Select(a => new SelectListItem { Text = $"{a.Code} {a.Name}", Value = a.Id.ToString(), Selected = (groupClaim == a.Id.ToString()) }).ToListAsync();
        }
        private async Task<IEnumerable<SelectListItem>> GetSubjectClaimSelectList(string groupClaim, string subjectClaim)
        {
            return await subjectClaimRepository.Filter(groupClaim: groupClaim).Select(a => new SelectListItem { Text = $"{a.Code} {a.Name}", Value = a.Id.ToString(), Selected = (subjectClaim == a.Id.ToString()) }).ToListAsync();
        }
        private async Task<IEnumerable<SelectListItem>> GetСourtSelectList(string court)
        {
            return await dirRepository.Filter(lord: "Суд").Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (court == a.Id.ToString()) }).OrderBy(a => a.Text).ToListAsync();
        }
        private async Task<IEnumerable<SelectListItem>> GetPerformerSelectList(string district, string performer)
        {
            var p = new List<Performer>();
            var districts = await districtRepository.GetByIdWithItemsAsync(district.ToLong());
            if (districts != null)
            {
                var districtPerformers = districts.DistrictPerformers;
                foreach (var dp in districtPerformers)
                {
                    p.Add(dp.Performer);
                }
            }
            return p.OrderBy(n => n.Name).Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (performer == a.Id.ToString()) });
        }
        private async Task<IEnumerable<SelectListItem>> GetApplicantSelectList()
        {
            return await applicantRepository.Filter().OrderBy(a => a.Name).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = string.Concat(a.Name, " ", a.Address) }).ToListAsync();
        }
        #endregion
        #region Details
        // GET: Claims/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
                //throw new ApplicationException($"Не удалось загрузить район с ID {id}.");
            }

            var model = new ItemViewModel { Id = item.Id, Code = item.Code, Name = item.Name, NumPFR = item.NumPFR, Description = item.Description, /*RegionId = item.RegionId,*/ Region = item.Region, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, District = item.District, Instances = item.Instances, Meetings = item.Meetings, FileEntities = item.FileEntities, DateReg = item.DateReg, DateIn = item.DateIn, CategoryDisputeId = item.CategoryDisputeId, CategoryDispute = item.CategoryDispute, GroupClaim = item.GroupClaim, SubjectClaim = item.SubjectClaim };
            return View(model);
        }
        #endregion
        #region Validate
        public JsonResult ValidateDate(string DateReg)
        {
            DateTime parsedDate;
            DateTime MinDate;
            if(User.IsInRole(Role.AdminOPFR)) {
                return Json(true);
            }
            if (((((DateTime.Now.Month % 3)==1)&&(DateTime.Now.Day>14))||(((DateTime.Now.Month % 3) == 2) && (DateTime.Now.Day == 1)))){
                if (DateTime.Now.Month < 4)
                {
                    MinDate = new DateTime(DateTime.Now.Year - 1, DateTime.Now.Month + (-3 + 12), 1);
                }
                else
                {
                    MinDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 3, 1);
                }
                if (!DateTime.TryParse(DateReg, out parsedDate))
                {
                    return Json("Пожалуйста, введите дату в формате (дд.мм.гггг)");//, JsonRequestBehavior.AllowGet);
                }
                else if (MinDate > parsedDate)
                {
                    return Json($"Введите дату позже {MinDate}");//,                    JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(true);//, JsonRequestBehavior.AllowGet);
                }
            }else return Json(true);
        }
        #endregion
        #region Create
        // GET: Claims/Create
        [AuthorizeRoles(Role.AdminUPFR, Role.UserUPFR, Role.Administrator)]
        public async Task<IActionResult> Create(string lord = null, string owner = null)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            if (string.IsNullOrWhiteSpace(lord))
            {
                lord = user.RegionId.ToString();
            }
            if (string.IsNullOrWhiteSpace(owner))
            {
                owner = user.DistrictId.ToString();
            }

            var model = new CreateViewModel
            {
                RegionId = (long)lord.ToLong(),
                DistrictId = (long)owner.ToLong(),
                Regions = await GetRegionSelectList(lord),
                Districts = await GetDistrictSelectList(lord, owner),
                DateReg = DateTime.Now
            };
            return View(model);
        }
        // POST: Claims/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Role.AdminUPFR, Role.UserUPFR, Role.Administrator)]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var item = await repository.AddAsync(new Claim { Code = model.Name.GetCode(model.DateReg, model.CreatedOnUtc.Year, model.Id)/* $"{model.Id}-{DateTime.Now.Year.ToString()}-{model.Name}/{model.DateReg.ToString("D")}"*/, Name = model.Name, Description = model.Description, RegionId = model.RegionId, DistrictId = model.DistrictId, DateReg = model.DateReg });
                if (item != null)
                {
                    StatusMessage = item.MessageAddOk();
                    logger.LogInformation($"{model} создано");
                    return RedirectToAction(nameof(Edit), new { id = item.Id });
                }
            }
            //ModelState.AddModelError(string.Empty, model.MessageAddError());
            model.Regions = await GetRegionSelectList(model.RegionId.ToString());
            model.Districts = await GetDistrictSelectList(model.RegionId.ToString(), model.DistrictId.ToString());
            return View(model);
        }
        #endregion
        #region Edit
        // GET: Claims/Edit/5
        [AuthorizeRoles(Role.AdminUPFR, Role.UserUPFR, Role.Administrator)]
        public async Task<ActionResult> Edit(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }
            var model = new EditViewModel { Id = item.Id, Code = item.Code, Name = item.Name, NumPFR = item.NumPFR, Description = item.Description, RegionId = item.RegionId, Regions = await GetRegionSelectList(item.RegionId.ToString()), StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, DistrictId = item.DistrictId, Districts = await GetDistrictSelectList(item.RegionId.ToString(), item.DistrictId.ToString()), DateReg = item.DateReg, DateIn = item.DateIn, CategoryDisputeId = item.CategoryDisputeId, CategoryDisputes = await GetCategoreSelectList(item.CategoryDisputeId.ToString()), GroupClaimId = item.GroupClaimId, GroupClaims = await GetGroupClaimSelectList(item.CategoryDisputeId.ToString(), item.GroupClaimId.ToString()), SubjectClaimId = item.SubjectClaimId, SubjectClaims = await GetSubjectClaimSelectList(item.GroupClaimId.ToString(), item.SubjectClaimId.ToString()), СourtId = item.СourtId, Сourts = await GetСourtSelectList(item.СourtId.ToString()), PerformerId = item.PerformerId, Performers = await GetPerformerSelectList(item.DistrictId.ToString(), item.PerformerId.ToString()), Applicants = await GetApplicantSelectList(), Sum = item.Sum, PlaintiffId = item.PlaintiffId, RespondentId = item.RespondentId, Person3rdId = item.Person3rdId };
            return View(model);
        }
        // POST: Claims/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Role.AdminUPFR, Role.UserUPFR, Role.Administrator)]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Code = model.Name.GetCode(model.DateReg, model.CreatedOnUtc.Year, model.Id); //$"{model.Id}-{model.CreatedOnUtc.Year}-{model.Name}/{model.DateReg.ToString("D")}";
                    if ((model.RegionId != 0) && (model.DistrictId != 0))
                    {
                        await repository.UpdateAsync(new Claim { Id = model.Id, Code = model.Code, Description = model.Description, Name = model.Name, NumPFR = model.NumPFR, CreatedOnUtc = model.CreatedOnUtc, CategoryDisputeId = model.CategoryDisputeId, RegionId = model.RegionId, DistrictId = model.DistrictId, DateReg = model.DateReg, DateIn = model.DateIn, GroupClaimId = model.GroupClaimId, SubjectClaimId = model.SubjectClaimId, СourtId = model.СourtId, PerformerId = model.PerformerId, Sum = model.Sum, PlaintiffId = model.PlaintiffId, RespondentId = model.RespondentId, Person3rdId = model.Person3rdId });
                        StatusMessage = model.MessageEditOk();
                        logger.LogInformation($"{model} изменено");
                    }
                    else { StatusMessage = $"Проверте заполнение полей"; }
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
            }
            model.Regions = await GetRegionSelectList(model.RegionId.ToString());
            model.Districts = await GetDistrictSelectList(model.RegionId.ToString(), model.DistrictId.ToString());
            model.CategoryDisputes = await GetCategoreSelectList(model.CategoryDisputeId.ToString());
            model.GroupClaims = await GetGroupClaimSelectList(model.CategoryDisputeId.ToString(), model.GroupClaimId.ToString());
            model.SubjectClaims = await GetSubjectClaimSelectList(model.GroupClaimId.ToString(), model.SubjectClaimId.ToString());
            model.Сourts = await GetСourtSelectList(model.СourtId.ToString());
            model.Performers = await GetPerformerSelectList(model.DistrictId.ToString(), model.PerformerId.ToString());
            model.Applicants = await GetApplicantSelectList();
            model.StatusMessage = StatusMessage;
            return View(model);
        }
        #endregion
        #region Delete
        // GET: Claims/Delete/5
        [AuthorizeRoles(Role.AdminUPFR, Role.Administrator)]
        public async Task<IActionResult> Delete(long? id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                return RedirectToAction(nameof(Index));
            }
            var model = new ItemViewModel { Id = item.Id, Code = item.Code, Name = item.Name, NumPFR = item.NumPFR, Description = item.Description, CategoryDisputeId = item.CategoryDisputeId, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, StatusMessage = StatusMessage, RegionId = item.RegionId, DistrictId = item.DistrictId };
            return View(model);
        }
        // POST: Claims/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Role.AdminUPFR, Role.Administrator)]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await repository.DeleteAsync(new Claim { Id = model.Id, Name = model.Name, Code = model.Code, });
                StatusMessage = model.MessageDeleteOk();
                logger.LogInformation($"{model} удалено");
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
