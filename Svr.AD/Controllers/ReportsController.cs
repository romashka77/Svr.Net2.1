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
using Svr.Utils;

//using OfficeOpenXml.Table;

namespace Svr.AD.Controllers
{
    //https://zennolab.com/discussion/threads/generacija-krasivyx-excel-otchjotov-po-shablonu.33585/

    //https://habr.com/ru/post/109820/
    //http://www.pvsm.ru/programmirovanie/49187#begin

    //https://riptutorial.com/ru/epplus/example/26411/text-alignment-and-word-wrap
    //https://ru.inettools.net/image/opredelit-tsvet-piksela-na-kartinke-onlayn
    //https://stackoverflow.com/questions/3604562/download-file-of-any-type-in-asp-net-mvc-using-fileresult
    [AuthorizeRoles(Role.Admin, Role.Users, Role.Manager)]
    public class ReportsController : MessageController
    {
        private const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private readonly IHostingEnvironment hostingEnvironment;
        private FileInfo template;
        private const string ReportsFolder = "Reports";
        private const string TemplatesFolder = "Templates";
        private const string FileTemplateNameOut = "0901.xlsx"; //"Template1.xlsx";
        private const string FileTemplateNameIn = "0902.xlsx";
        //private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<ReportsController> logger;
        private readonly IRegionRepository regionRepository;
        private readonly IDistrictRepository districtRepository;

        private readonly ICategoryDisputeRepository categoryDisputeRepository;
        private readonly IGroupClaimRepository groupClaimRepository;
        private readonly IClaimRepository claimRepository;
        private readonly IInstanceRepository instanceRepository;
        #region Конструктор
        public ReportsController(IHostingEnvironment hostingEnvironment, /*UserManager<ApplicationUser> userManager,*/
            ILogger<ReportsController> logger, IDistrictRepository districtRepository,
            IRegionRepository regionRepository, ICategoryDisputeRepository categoryDisputeRepository,
            IGroupClaimRepository groupClaimRepository, IClaimRepository claimRepository,
            IInstanceRepository instanceRepository)
        {
            this.logger = logger;
            //this.userManager = userManager;
            this.regionRepository = regionRepository;
            this.districtRepository = districtRepository;
            this.hostingEnvironment = hostingEnvironment;

            this.categoryDisputeRepository = categoryDisputeRepository;
            this.groupClaimRepository = groupClaimRepository;
            this.claimRepository = claimRepository;
            this.instanceRepository = instanceRepository;
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
            using (var package = await CreateExcelPackage(owner, dateS, datePo, category))
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
                owner = "24";//user.DistrictId.ToString();
            }
            var path = await GetPath(lord.ToLong(), owner.ToLong());
            //byte[] reportBytes;
            using (var package = await CreateExcelPackage(owner, dateS, datePo, category))
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
        #region GetFileTemplateName
        private async Task<FileInfo> GetFileTemplateName(string category)
        {
            string fileTemplateName;
            if (category.ToLong() == null)
            {
                StatusMessage = "Ошибка: Выберите категорию.";
                return null;
            }

            if ((await categoryDisputeRepository.GetByIdAsync(category.ToLong())).Name.ToUpper()
                .Equals("Входящие".ToUpper()))
                fileTemplateName = FileTemplateNameIn;
            else if ((await categoryDisputeRepository.GetByIdAsync(category.ToLong())).Name.ToUpper()
                .Equals("Исходящие".ToUpper()))
                fileTemplateName = FileTemplateNameOut;
            else
            {
                StatusMessage = "Ошибка: Категория не определена.";
                return null;
            }
            var fileInfo = new FileInfo(Path.Combine(hostingEnvironment.WebRootPath, TemplatesFolder, fileTemplateName));
            if (fileInfo.Exists) return fileInfo;
            StatusMessage = $"Ошибка: Файл Excel-шаблона {fileTemplateName} отсутствует.";
            return null;
        }
        #endregion
        

        private async Task<ExcelPackage> CreateExcelPackage(string owner = null,
            DateTime? dateS = null, DateTime? datePo = null, string category = null)
        {
            const int MAX0901 = 18;
            const int MAX0902 = 25;

            const int I0901 = 4;
            const int I0902 = 5;

            template = await GetFileTemplateName(category);
            if (template == null) return null;
            var package = new ExcelPackage(template, true);
            package.Workbook.Properties.Author = User.Identity.Name;
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            var worksheetLog = package.Workbook.Worksheets.Add("log");
            //Группы споров
            var groupClaims =
                    (await groupClaimRepository.ListAsync(new GroupClaimSpecificationReport(category.ToLong())))
                    .OrderBy(a => a.Code.ToLong());
            //Гос.пошлина
            var duty = ExcelEx.InitialRecord();
            //Услуги пред.
            var services = ExcelEx.InitialRecord();
            //Суд.издержки
            var cost = ExcelEx.InitialRecord();

            var dutyPaid = ExcelEx.InitialRecord();

            var cellslog = worksheetLog.Cells;
            int l = 1;
            ExcelEx.InitLog(cellslog, l);
            worksheetLog.Column(l + 1).Width = 50;

            for (int i = 0; i < 12; i++)
            {
                if ((i % 3) == 0)
                    worksheetLog.Column(i + l + 3).Width = 50;
            }

            foreach (var groupClaim in groupClaims)
            {
                cellslog[$"A{l}"].Value = $"{ groupClaim.Code}";
                cellslog[$"B{l++}"].Value = $"{groupClaim.Name}";
                byte flg = 0;
                long? groupClaimCode = groupClaim.Code.ToLong();
                if ((template.Name.Equals(FileTemplateNameIn) && (groupClaimCode > 0) && (groupClaimCode < I0902)) || (template.Name.Equals(FileTemplateNameOut) && (groupClaimCode > 0) && (groupClaimCode < I0901)))
                {
                    flg = 1;
                }
                else if ((template.Name.Equals(FileTemplateNameIn) && (groupClaimCode > (I0902 - 1)) && (groupClaimCode <= MAX0902)) || (template.Name.Equals(FileTemplateNameOut) && (groupClaimCode > (I0901 - 1)) && (groupClaimCode <= MAX0901)))
                {
                    flg = 2;
                }
                /// Предметы иска
                var subjectClaims = groupClaim.SubjectClaims.OrderBy(a => a.Code);
                foreach (var subjectClaim in subjectClaims)
                {
                    try
                    {
                        cellslog[$"A{l}"].Value = $"{ subjectClaim.Code}";
                        cellslog[$"B{l++}"].Value = $"{subjectClaim.Name}";
                        var groupRecord = ExcelEx.InitialRec();
                        var claims = claimRepository.List(new ClaimSpecificationReport(owner.ToLong())).Where(c => c.SubjectClaimId == subjectClaim.Id);
                        var n = (from cell in worksheet.Cells["A:A"] where cell.Text.Equals(subjectClaim.Code) select cell)
                            ?.Last().End.Row;
                        if (n == null) continue;

                        if (dateS != null)
                        {
                            claims = claims.Where(c => c.DateIn >= dateS);
                        }
                        if (datePo != null)
                        {
                            claims = claims.Where(c => c.DateIn <= datePo);
                        }
                        var count = await claims.CountAsync();
                        if (count > 0)
                        {
                            foreach (var claim in claims)
                            {
                                cellslog[$"B{l}"].Value = $"{ claim.Code}";//{ claim.Name}
                                cellslog[$"C{l++}"].Value = $"{ claim.Sum}";
                            }
                            groupRecord[0].Count += count;
                            var sum = await claims.SumAsync(c => c.Sum);
                            if (sum != null)
                            {
                                groupRecord[0].Sum += (sum ?? 0);
                            }
                        }
                        var instances = instanceRepository.ListReport().Where(i => i.Claim.SubjectClaimId == subjectClaim.Id);
                        if (!String.IsNullOrWhiteSpace(owner))
                        {
                            instances = instances.Where(i => i.Claim.DistrictId == owner.ToLong());
                        }
                        //instances = instances.Where(i => i.ClaimId == itemp);
                        if (dateS != null)
                        {
                            instances = instances.Where(c => c.DateInCourtDecision >= dateS);
                        }
                        if (datePo != null)
                        {
                            instances = instances.Where(c => c.DateInCourtDecision <= datePo);
                        }
                        //-------------------------------
                        var instancesMax = instances.GroupBy(p => p.ClaimId, p => p.Number, (key, g) => new { ClaimsId = key, MaxNumber = g.Max() });
                        instances = instances.Join(instancesMax, p => p.ClaimId, t => t.ClaimsId, (p, t) => new
                        {
                            p.Claim,
                            p.ClaimId,
                            p.CourtDecision,
                            p.CourtDecisionId,
                            p.CreatedOnUtc,
                            p.DateCourtDecision,
                            p.DateInCourtDecision,
                            p.DateTransfer,
                            p.Description,
                            p.DutyDenied,
                            p.DutyPaid,
                            p.DutySatisfied,
                            p.Id,
                            p.Name,
                            p.Number,
                            p.ServicesDenied,
                            p.ServicesSatisfied,
                            p.SumDenied,
                            p.SumSatisfied,
                            p.UpdatedOnUtc,
                            p.СostDenied,
                            p.СostSatisfied,
                            MaxNumber = t.MaxNumber
                        }).Where(r => r.Number == r.MaxNumber).Select(p => new Instance
                        {
                            Claim = p.Claim,
                            ClaimId = p.ClaimId,
                            CourtDecision = p.CourtDecision,
                            CourtDecisionId = p.CourtDecisionId,
                            CreatedOnUtc = p.CreatedOnUtc,
                            DateCourtDecision = p.DateCourtDecision,
                            DateInCourtDecision = p.DateInCourtDecision,
                            DateTransfer = p.DateTransfer,
                            Description = p.Description,
                            DutyDenied = p.DutyDenied,
                            DutyPaid = p.DutyPaid,
                            DutySatisfied = p.DutySatisfied,
                            Id = p.Id,
                            Name = p.Name,
                            Number = p.Number,
                            ServicesDenied = p.ServicesDenied,
                            ServicesSatisfied = p.ServicesSatisfied,
                            SumDenied = p.SumDenied,
                            SumSatisfied = p.SumSatisfied,
                            UpdatedOnUtc = p.UpdatedOnUtc,
                            СostDenied = p.СostDenied,
                            СostSatisfied = p.СostSatisfied
                        });
                        //-------------------------------
                        var instances1 = await instances.Where(i => i.Number == 1).AsNoTracking().ToListAsync();
                        if (instances1.Count > 0)
                        {
                            foreach (var instance in instances1)
                            {
                                cellslog[$"D{l}"].Value = $"{ instance.Name} {instance.Claim.Code}";
                                cellslog[$"E{l}"].Value = $"{ instance.SumSatisfied}";
                                cellslog[$"F{l++}"].Value = $"{ instance.SumDenied}";
                            }
                            var k = 0;
                            if (flg == 1)
                                k = 1;
                            else if (flg == 2)
                                k = 2;
                            ExcelEx.GetSumInstances(instances1, groupRecord[1], groupRecord[2], groupRecord[9], groupRecord[10], duty[k], services[k], cost[k], dutyPaid[k]);
                        }
                        var instances2 = await instances.Where(i => i.Number == 2).AsNoTracking().ToListAsync();
                        if (instances2.Count > 0)
                        {
                            foreach (var instance in instances2)
                            {
                                cellslog[$"G{l}"].Value = $"{ instance.Name} {instance.Claim.Code}";
                                cellslog[$"H{l}"].Value = $"{ instance.SumSatisfied}";
                                cellslog[$"I{l++}"].Value = $"{ instance.SumDenied}";
                            }
                            var k = 0;
                            if (flg == 1)
                                k = 3;
                            else if (flg == 2)
                                k = 4;
                            ExcelEx.GetSumInstances(instances2, groupRecord[3], groupRecord[4], groupRecord[9], groupRecord[10], duty[k], services[k], cost[k], dutyPaid[k]);
                        }
                        var instances3 = await instances.Where(i => i.Number == 3).AsNoTracking().ToListAsync();
                        if (instances3.Count > 0)
                        {
                            foreach (var instance in instances3)
                            {
                                cellslog[$"J{l}"].Value = $"{ instance.Name} {instance.Claim.Code}";
                                cellslog[$"K{l}"].Value = $"{ instance.SumSatisfied}";
                                cellslog[$"L{l++}"].Value = $"{ instance.SumDenied}";
                            }
                            var k = 0;
                            if (flg == 1)
                                k = 5;
                            else if (flg == 2)
                                k = 6;
                            ExcelEx.GetSumInstances(instances3, groupRecord[5], groupRecord[6], groupRecord[9], groupRecord[10], duty[k], services[k], cost[k], dutyPaid[k]);
                        }
                        var instances4 = await instances.Where(i => i.Number == 4).AsNoTracking().ToListAsync();
                        if (instances4.Count > 0)
                        {
                            foreach (var instance in instances4)
                            {
                                cellslog[$"M{l}"].Value = $"{ instance.Name} {instance.Claim.Code}";
                                cellslog[$"N{l}"].Value = $"{ instance.SumSatisfied}";
                                cellslog[$"O{l++}"].Value = $"{ instance.SumDenied}";
                            }
                            var k = 0;
                            if (flg == 1)
                                k = 7;
                            else if (flg == 2)
                                k = 8;
                            ExcelEx.GetSumInstances(instances4, groupRecord[7], groupRecord[8], groupRecord[9], groupRecord[10], duty[k], services[k], cost[k], dutyPaid[k]);
                        }
                        ExcelEx.SetCells2(worksheet, groupRecord, subjectClaim.Code);
                        //groupRecord = null;
                    }
                    catch (Exception e)
                    {
                        logger.LogError($"{e.Message}  subjectClaim.Code = {subjectClaim.Code}");
                        //Console.WriteLine($"{e.Message}  subjectClaim.Code = {subjectClaim.Code}");
                        throw;
                    }
                }
            }
            ExcelEx.SetCells(worksheet, worksheetLog, duty, ref l, (template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}.1" : $"{MAX0901}.1"));
            ExcelEx.SetCells(worksheet, worksheetLog, dutyPaid, ref l, (template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}.1" : $"{MAX0901}.1"));
            ExcelEx.SetCells(worksheet, worksheetLog, services, ref l, (template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}.2" : ""));
            ExcelEx.SetCells(worksheet, worksheetLog, cost, ref l, (template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}.3" : $"{MAX0901}.2"));


            ExcelEx.SetCells(worksheet, worksheetLog, duty, ref l, (template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}.4" : $"{MAX0901}.3"), 1);
            ExcelEx.SetCells(worksheet, worksheetLog, dutyPaid, ref l, (template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}.4" : $"{MAX0901}.3"), 1);
            ExcelEx.SetCells(worksheet, worksheetLog, services, ref l, (template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}.5" : ""), 1);
            ExcelEx.SetCells(worksheet, worksheetLog, cost, ref l, (template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}.5" : $"{MAX0901}.4"), 1);


            ExcelEx.SetCells(worksheet, worksheetLog, duty, ref l, (template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}" : $"{MAX0901}"));
            ExcelEx.SetCells(worksheet, worksheetLog, dutyPaid, ref l, (template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}" : $"{MAX0901}"));
            ExcelEx.SetCells(worksheet, worksheetLog, services, ref l, (template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}" : ""));
            ExcelEx.SetCells(worksheet, worksheetLog, cost, ref l, (template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}" : $"{MAX0901}"));

            ExcelEx.SetCells(worksheet, worksheetLog, duty, ref l, (template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}" : $"{MAX0901}"), 1);
            ExcelEx.SetCells(worksheet, worksheetLog, dutyPaid, ref l, (template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}" : $"{MAX0901}"), 1);
            ExcelEx.SetCells(worksheet, worksheetLog, services, ref l, (template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}" : ""), 1);
            ExcelEx.SetCells(worksheet, worksheetLog, cost, ref l, (template.Name.Equals(FileTemplateNameIn) ? $"{MAX0902}" : $"{MAX0901}"), 1);

            return package;
        }

        private string GetFileName(DateTime? dateS, DateTime? datePo)
        {
            return $"{dateS?.ToString("yyyy.MM.dd")}-{datePo?.ToString("yyyy.MM.dd")} {template.Name}";
        }

        private async Task<string> GetPath(long? lord, long? owner)
        {
            var path = Path.Combine(hostingEnvironment.WebRootPath, ReportsFolder);
            if (lord != null)
            {
                var region = await regionRepository.GetByIdAsync(lord);
                path = Path.Combine(path, region.Name);
            }
            if (owner != null)
            {
                var district = await districtRepository.GetByIdAsync(owner);
                path = Path.Combine(path, district.Name);
            }
            return path;
        }
    }
}