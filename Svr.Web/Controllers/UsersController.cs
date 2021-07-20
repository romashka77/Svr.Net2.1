using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Infrastructure.Data;
using Svr.Infrastructure.Extensions;
using Svr.Infrastructure.Identity;
using Svr.Utils.Controllers;
using Svr.Utils.Models;
using Svr.Utils.Roles;
using Svr.Web.Models.UsersViewModels;

namespace Svr.Web.Controllers
{
	[AuthorizeRoles(Role.Administrator, Role.AdminOPFR, Role.AdminUPFR)]
	public class UsersController : MessageController
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly ILogger<UsersController> logger;
		private readonly IDistrictRepository districtRepository;
		private readonly IRegionRepository regionRepository;
		#region Конструктор
		public UsersController(ILogger<UsersController> logger, UserManager<ApplicationUser> userManager, IRegionRepository regionRepository, IDistrictRepository districtRepository)
		{
			this.logger = logger;
			this.userManager = userManager;
			this.regionRepository = regionRepository;
			this.districtRepository = districtRepository;
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
		public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string lord = null, string owner = null, string searchString = null, int page = 1, int itemsPage = 5)
		{
			var user = await userManager.FindByNameAsync(User.Identity.Name);
			var list = userManager.Users;
			if (User.IsInRole(Role.Administrator))
			{ }
			else
			if (User.IsInRole(Role.AdminOPFR))
			{
				list = list.Where(i => i.RegionId == user.RegionId || i.RegionId == null);
			}
			else
			if (User.IsInRole(Role.AdminUPFR))
			{
				list = list.Where(i => i.DistrictId == user.DistrictId || i.RegionId == null);
			}
			//фильтрация
			if (!string.IsNullOrWhiteSpace(searchString))
			{
				var upsearch = searchString.ToUpper();
				list = list.Where(d => d.Email.ToUpper().Contains(upsearch) || d.LastName.ToUpper().Contains(upsearch) || d.FirstName.ToUpper().Contains(upsearch) || d.MiddleName.ToUpper().Contains(upsearch));

			}

			switch (sortOrder)
			{
				case SortState.NameDesc:
				list.OrderByDescending(p => p.Email);
				break;
				case SortState.NameAsc:
				list.OrderBy(s => s.Email);
				break;
				default:
				list.OrderBy(s => s.Email);
				break;
			}

			var itemsCount = await list.CountAsync();






			var itemsOnPage = await list.Skip((page - 1) * itemsPage).Take(itemsPage).AsNoTracking().ToListAsync();
			var indexModel = new IndexViewModel()
			{
				ItemViewModels = itemsOnPage.Select(i => new ItemViewModel()
				{
					Id = i.Id,
					LastName = i.LastName,
					FirstName = i.FirstName,
					MiddleName = i.MiddleName,
					Email = i.Email,
					PhoneNumber = i.PhoneNumber,
					//CreatedOnUtc = i.CreatedOnUtc,
					//UpdatedOnUtc = i.UpdatedOnUtc,

				}),
				PageViewModel = new PageViewModel(itemsCount, page, itemsPage),
				SortViewModel = new SortViewModel(sortOrder),
				FilterViewModel = new FilterViewModel(searchString: searchString, owner: owner, owners: await GetDistrictSelectList(lord, owner), lord: lord, lords: await GetRegionSelectList(lord), itemsCount: itemsCount),


				StatusMessage = StatusMessage
			};
			return View(indexModel);
		}
		#endregion
		#region Delete
		// GET: Users/Delete/5
		[AuthorizeRoles(Role.Administrator)]
		public async Task<IActionResult> Delete(string id)
		{
			var item = await userManager.FindByIdAsync(id);
			if (item == null)
			{
				StatusMessage = id.ErrorFind();
				return RedirectToAction(nameof(Index));
			}
			var model = new ItemViewModel { Id = item.Id, LastName = item.LastName, FirstName = item.FirstName, MiddleName = item.MiddleName, Email = item.Email, PhoneNumber = item.PhoneNumber, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, StatusMessage = StatusMessage, RegionId = item.RegionId, DistrictId = item.DistrictId };
			return View(model);
		}
		// POST: Users/Delete/
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[AuthorizeRoles(Role.AdminUPFR, Role.Administrator)]
		public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
		{
			try
			{
				//await userManager.DeleteAsync(new ApplicationUser { Id = model.Id, LastName = model.LastName, FirstName = model.FirstName, MiddleName = model.MiddleName, Email = model.Email, });
				await userManager.DeleteAsync(await userManager.FindByIdAsync(model.Id));

				//StatusMessage = model.MessageDeleteOk();
				logger.LogInformation($"{model} удалено");
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				//StatusMessage = $"{model.MessageDeleteError()} {ex.Message}.";
				return RedirectToAction(nameof(Index));
			}
		}
		#endregion
		#region Details
		// GET: Users/Details/string
		public async Task<IActionResult> Details(string id)
		{
			var item = await userManager.FindByIdAsync(id);
			if (item == null)
			{
				StatusMessage = id.ToString().ErrorFind();
				return RedirectToAction(nameof(Index));
				//throw new ApplicationException($"Не удалось загрузить район с ID {id}.");
			}
			var model = new ItemViewModel { Id = item.Id, Email = item.Email, FirstName = item.FirstName, LastName = item.LastName, MiddleName = item.MiddleName, PhoneNumber = item.PhoneNumber, RegionId = item.RegionId, Region = (await regionRepository.GetByIdAsync(item.RegionId)).Name, DistrictId = item.DistrictId, District = (await districtRepository.GetByIdAsync(item.DistrictId)).Name, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, };
			return View(model);
		}
		#endregion
		private async Task<IEnumerable<SelectListItem>> GetRegionSelectList(string lord)
		{
			return await regionRepository.Filter(lord: lord, flgFilter: !User.IsInRole(Role.Administrator)).Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (lord == a.Id.ToString()) }).OrderBy(a => a.Text).ToListAsync();
		}
		private async Task<IEnumerable<SelectListItem>> GetDistrictSelectList(string lord, string owner)
		{
			return await districtRepository.Filter(lord: lord, owner: owner, flgFilter: (User.IsInRole(Role.AdminUPFR) || User.IsInRole(Role.UserUPFR))).Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) }).OrderBy(a => a.Text).ToListAsync();
		}

	}
}