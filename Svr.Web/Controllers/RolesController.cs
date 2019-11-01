using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Identity;
using Svr.Web.Models;
using Svr.Web.Models.RoleViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    [Authorize(Roles = "Администратор ОПФР, Администратор УПФР, Администратор")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private IDistrictRepository districtRepository;
        private IRegionRepository regionRepository;
        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IDistrictRepository districtRepository, IRegionRepository regionRepository)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.regionRepository = regionRepository;
            this.districtRepository = districtRepository;
        }
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                districtRepository = null;
                regionRepository = null;
            }
            base.Dispose(disposing);
        }
        #endregion

        public IActionResult Index() => View(roleManager.Roles.ToList());
        [Authorize(Roles = "Администратор")]
        public IActionResult Create() => View();
        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Create(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(name);
        }

        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role != null)
            {
                await roleManager.DeleteAsync(role);
            }
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Администратор ОПФР, Администратор УПФР, Администратор")]
        public async Task<IActionResult> UserList()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var list = userManager.Users;
            if (User.IsInRole("Администратор"))
            { }
            else
            if (User.IsInRole("Администратор ОПФР"))
            {
                list = list.Where(i => i.RegionId == user.RegionId);
            }
            else
            if (User.IsInRole("Администратор УПФР"))
            {
                list = list.Where(i => i.DistrictId == user.DistrictId);
            }
            return View(list.ToList());
        }
        private async Task<IEnumerable<SelectListItem>> GetRegionSelectList(string lord)
        {
            return await regionRepository.Filter(lord: lord, flgFilter: !User.IsInRole("Администратор")).Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (lord == a.Id.ToString()) }).OrderBy(a => a.Text).ToListAsync();
        }
        private async Task<IEnumerable<SelectListItem>> GetDistrictSelectList(string lord, string owner)
        {
            return await districtRepository.Filter(lord: lord, owner: owner, flgFilter: (User.IsInRole("Администратор УПФР") || User.IsInRole("Пользователь УПФР"))).Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) }).OrderBy(a => a.Text).ToListAsync();
        }
        public async Task<IActionResult> Edit(string userId)
        {
            // получаем пользователя
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = await userManager.GetRolesAsync(user);
                var allRoles = roleManager.Roles;
                if (User.IsInRole("Администратор"))
                { }
                else
                if (User.IsInRole("Администратор ОПФР"))
                {
                    allRoles = allRoles.Where(i => i.Name.Contains("Пользователь ")|| i.Name.Contains("Администратор УПФР"));
                }
                else
                if (User.IsInRole("Администратор УПФР"))
                {
                    allRoles = allRoles.Where(i => i.Name.Contains("Пользователь УПФР"));
                }

                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRoles = userRoles,
                    AllRoles = allRoles.ToList(),
                    //FilterViewModel = new FilterViewModel(searchString: searchString, owner: owner, owners: await GetDistrictSelectList(lord, owner), lord: lord, lords: await GetRegionSelectList(lord), dateS: dateS, datePo: datePo, category: category, categores: await GetCategoreSelectList(category), groupClaim: groupClaim, groupClaims: await GetGroupClaimSelectList(category, groupClaim), subjectClaim: subjectClaim, subjectClaims: await GetSubjectClaimSelectList(groupClaim, subjectClaim), resultClaim: resultClaim, resultClaims: await GetResultClaimSelectList(), itemsCount: itemsCount)
                    ////ViewBag.Districts = new SelectList(await districtRepository.ListAsync(new DistrictSpecification
                };
                ViewBag.Regions = new SelectList(await regionRepository.ListAllAsync(), "Id", "Name", model.RegionId);
                ViewBag.Districts = new SelectList(await districtRepository.ListAsync(new DistrictSpecification(null)), "Id", "Name", model.DistrictId);
                return View(model);
            }
            return NotFound();
        }

        public async Task<IActionResult> ResetPassword(string userId)
        {
            ApplicationUser user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await userManager.RemovePasswordAsync(user);
                await userManager.AddPasswordAsync(user, "Test123456789");
                return RedirectToAction(nameof(UserList));
            }
            return NotFound();
        }



        [HttpPost]
        public async Task<IActionResult> Edit(string userId, List<string> roles, long? regionId, long? districtId)
        {
            // получаем пользователя
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();
            user.RegionId = regionId;
            user.DistrictId = districtId;
            await userManager.UpdateAsync(user);
            //await userManager.AccessFailedAsync(user);
            // получем список ролей пользователя
            var userRoles = await userManager.GetRolesAsync(user);
            // получаем все роли
            //var allRoles = roleManager.Roles.ToList()
            // получаем список ролей, которые были добавлены
            var addedRoles = roles.Except(userRoles);
            // получаем роли, которые были удалены
            var removedRoles = userRoles.Except(roles);
            await userManager.AddToRolesAsync(user, addedRoles);
            await userManager.RemoveFromRolesAsync(user, removedRoles);
            return RedirectToAction(nameof(UserList));
        }
    }
}