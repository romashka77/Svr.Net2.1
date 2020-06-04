using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Extensions;
using Svr.Infrastructure.Identity;
using Svr.AD.Extensions;
using Svr.AD.Models;
using Svr.AD.Models.ReportsViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Svr.Utils.Roles;
using Svr.Utils.Controllers;

//using OfficeOpenXml.Table;

namespace Svr.AD.Controllers
{
    [AuthorizeRoles(Role.Admin, Role.Users, Role.Manager)]
    public class ReportsController : MessageReportController
    {
        //private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<ReportsController> logger;
        #region Конструктор
        public ReportsController(IHostingEnvironment hostingEnvironment, /*UserManager<ApplicationUser> userManager,*/
            ILogger<ReportsController> logger, IDistrictRepository districtRepository,
            IRegionRepository regionRepository, ICategoryDisputeRepository categoryDisputeRepository,
            IGroupClaimRepository groupClaimRepository, IClaimRepository claimRepository,
            IInstanceRepository instanceRepository):base(categoryDisputeRepository, hostingEnvironment, regionRepository, districtRepository, groupClaimRepository, claimRepository, instanceRepository)
        {
            this.logger = logger;
            //this.userManager = userManager;
        }

        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //districtRepository = null;
                //regionRepository = null;
                //logger = null;
            }

            base.Dispose(disposing);
        }

        #endregion
        #region Index
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string lord = null,
            string owner = null, string searchString = null, int page = 1, int itemsPage = 10, DateTime? dateS = null,
            DateTime? datePo = null, string category = null)
        {
            //var user = await userManager.FindByNameAsync(User.Identity.Name);

            lord = this.GetLord(lord);
            owner = this.GetOwner(owner);

            var path = await GetPath(lord.ToLong(), owner.ToLong());

            var dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            IEnumerable<FileInfo> list = dirInfo.GetFiles();
            //фильтрация
            if (!string.IsNullOrEmpty(searchString))
            {
                list = list.Where(d =>
                    d.Name.ToUpper().Contains(searchString.ToUpper()) ||
                    d.Extension.ToUpper().Contains(searchString.ToUpper()));
            }
            // сортировка
            list = list.Sort(sortOrder);

            // определение страниц
            // ReSharper disable once PossibleMultipleEnumeration
            var count = list.Count();
            // ReSharper disable once PossibleMultipleEnumeration
            var itemsOnPage = list.Skip((page - 1) * itemsPage).Take(itemsPage).ToList();
            var indexModel = new IndexViewModel
            {
                ItemViewModels = itemsOnPage.Select(i => new ItemViewModel
                {
                    Name = i.Name,
                    Code = i.Extension,
                    CreatedOnUtc = i.CreationTime,
                    UpdatedOnUtc = i.LastWriteTime
                }),
                PageViewModel = new PageViewModel(count, page, itemsPage),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(searchString, owner,
                    (await districtRepository.ListAsync(new DistrictSpecification(lord.ToLong()))).Select(a =>
                        new SelectListItem
                        { Text = a.Name, Value = a.Id.ToString(), Selected = (owner == a.Id.ToString()) }), lord,
                    new SelectList((await regionRepository.ListAllAsync()).OrderBy(n => n.Name), "Id", "Name", lord),

                    //(await regionRepository.ListAllAsync()).ToList().Select(a => new SelectListItem
                    //{ Text = a.Name, Value = a.Id.ToString(), Selected = (lord == a.Id.ToString()) }), 
                    dateS, datePo,
                     category,
                    (await categoryDisputeRepository.ListAllAsync()).Select(a => new SelectListItem
                    { Text = a.Name, Value = a.Id.ToString(), Selected = (category == a.Id.ToString()) })),
                StatusMessage = StatusMessage
            };
            return View(indexModel);
        }
        #endregion
        #region InMemoryReport
        public async Task<IActionResult> InMemoryReport(string lord = null, string owner = null, DateTime? dateS = null,
            DateTime? datePo = null, string category = null)
        {
            if (string.IsNullOrEmpty(owner) && string.IsNullOrEmpty(lord))
            {
                //var user = await userManager.FindByNameAsync(User.Identity.Name);
                //owner = "24";//user.DistrictId.ToString();
            }
            byte[] reportBytes;
            using (var package = await CreateExcelPackage(User.Identity.Name, owner, dateS, datePo, category))
            {
                if (package == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                reportBytes = package.GetAsByteArray();
            }
            return File(reportBytes, XlsxContentType, GetFileName(dateS, datePo));
        }
        #endregion
        #region FileReport
        public async Task<IActionResult> FileReport(string lord = null, string owner = null, DateTime? dateS = null,
            DateTime? datePo = null, string category = null)
        {
            if (string.IsNullOrEmpty(owner) && string.IsNullOrEmpty(lord))
            {
                //var user = await userManager.FindByNameAsync(User.Identity.Name);
                //owner = "24";//user.DistrictId.ToString();
            }
            var path = await GetPath(lord.ToLong(), owner.ToLong());
            //byte[] reportBytes;
            using (var package = await CreateExcelPackage(User.Identity.Name, owner, dateS, datePo, category))
            {
                if (package == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                //reportBytes = package.GetAsByteArray();
                package.SaveAs(new FileInfo(Path.Combine(path, GetFileName(dateS, datePo))));
            }

            return File( /*path*/ /*reportBytes*/Path.Combine(path, GetFileName(dateS, datePo)), XlsxContentType,
                GetFileName(dateS, datePo));
        }
        #endregion
    }
}