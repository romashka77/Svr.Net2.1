using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Extensions;
using Svr.Web.Extensions;
using Svr.Web.Models;
using Svr.Web.Models.FileEntityViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    [AuthorizeRoles(Role.AdminOPFR, Role.UserOPFR, Role.AdminUPFR, Role.UserUPFR, Role.Administrator)]
    public class FileEntitiesController : Controller
    {
        private const string FilesFolder = "Files";
        private readonly IFileEntityRepository repository;
        private readonly IClaimRepository сlaimRepository;
        private readonly ILogger<FileEntitiesController> logger;
        private readonly IHostingEnvironment hostingEnvironment;

        [TempData]
        public string StatusMessage { get; set; }
        #region Конструктор
        public FileEntitiesController(IFileEntityRepository repository, IClaimRepository сlaimRepository, IHostingEnvironment hostingEnvironment, ILogger<FileEntitiesController> logger)
        {
            this.logger = logger;
            this.repository = repository;
            this.сlaimRepository = сlaimRepository;
            this.hostingEnvironment = hostingEnvironment;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //сlaimRepository = null;
                //repository = null;
                //hostingEnvironment = null;
                //logger = null;
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Index
        // GET: FileEntities
        public async Task<IActionResult> Index(SortState sortOrder = SortState.NameAsc, string owner = null, string searchString = null, int page = 1, int itemsPage = 10, DateTime? date = null)
        {
            var list = repository.List(new FileEntitySpecification(owner.ToLong()));
            //фильтрация
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
                    Description = i.Description,
                    CreatedOnUtc = i.CreatedOnUtc,
                    UpdatedOnUtc = i.UpdatedOnUtc,
                    Claim = i.Claim,
                    Path = i.Path
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
        // GET: FileEntities/Details/5
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
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, Claim = item.Claim, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, ClaimId = item.ClaimId, Path = item.Path };
            return View(model);
        }
        #endregion
        #region Create
        // GET: FileEntities/Create
        public IActionResult Create(long owner)
        {
            ViewBag.Owner = owner;
            return View();
        }
        // POST: FileEntities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Create(ItemViewModel model/*, IFormFile uploadedFile*/)
        {
            if ((ModelState.IsValid) && (model.UploadedFile != null))
            {
                // путь к папке Files
                model.Path = DateTime.Now.Year.ToString();
                DirectoryInfo dirInfo = new DirectoryInfo(GetFile(model.Path));
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }
                model.Path = Path.Combine(model.Path, $"{model.ClaimId}_{model.UploadedFile.FileName}");
                model.Name = model.UploadedFile.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(GetFile(model.Path), FileMode.Create))
                {
                    await model.UploadedFile.CopyToAsync(fileStream);
                }
                var item = await repository.AddAsync(new FileEntity { Name = model.Name, ClaimId = model.ClaimId, Description = model.Description, Claim = model.Claim, Path = model.Path });
                if (item != null)
                {
                    StatusMessage = item.MessageAddOk();
                    return RedirectToAction(nameof(Index), new { owner = item.ClaimId });
                }
            }
            ModelState.AddModelError(string.Empty, model.MessageAddError());
            //await SetViewBag(model);
            return View(model);
        }
        #endregion
        public async Task<IActionResult> Download(string path)
        {
            if (path == null)
                throw new ApplicationException($"Проверте имя файла.");

            var memory = new MemoryStream();
            using (var stream = new FileStream(GetFile(path), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
                logger.LogInformation($"{path} create");
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(GetFile(path)));
        }

        #region Edit
        // GET: FileEntities/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            var item = await repository.GetByIdWithItemsAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                //return RedirectToAction(nameof(Index));
                throw new ApplicationException(id.ToString().ErrorFind());
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, StatusMessage = StatusMessage, CreatedOnUtc = item.CreatedOnUtc, Claim = item.Claim, ClaimId = item.ClaimId, Path = item.Path };
            //await SetViewBag(model);
            return View(model);
        }
        //// POST: FileEntities/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await repository.UpdateAsync(new FileEntity { Id = model.Id, Description = model.Description, Name = model.Name, CreatedOnUtc = model.CreatedOnUtc, ClaimId = model.ClaimId, Path = model.Path });
                    StatusMessage = model.MessageEditOk();
                    logger.LogInformation($"{model} edit");
                    return RedirectToAction(nameof(Index), new { owner = model.ClaimId });
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
            //await SetViewBag(model);
            return View(model);
        }
        #endregion
        #region Delete
        // GET: FileEntities/Delete/5
        [AuthorizeRoles(Role.AdminOPFR, Role.AdminUPFR, Role.Administrator)]
        public async Task<IActionResult> Delete(long? id)
        {
            var item = await repository.GetByIdAsync(id);
            if (item == null)
            {
                StatusMessage = id.ToString().ErrorFind();
                //return RedirectToAction(nameof(Index));
                throw new ApplicationException(id.ToString().ErrorFind());
            }
            var model = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, CreatedOnUtc = item.CreatedOnUtc, UpdatedOnUtc = item.UpdatedOnUtc, StatusMessage = StatusMessage, ClaimId = item.ClaimId, Path = item.Path };
            return View(model);
        }

        // POST: FileEntities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(Role.AdminOPFR, Role.AdminUPFR, Role.Administrator)]
        public async Task<IActionResult> DeleteConfirmed(ItemViewModel model)
        {
            try
            {
                await repository.DeleteAsync(new FileEntity { Id = model.Id, Name = model.Name });
                StatusMessage = model.MessageDeleteOk();

                var fileInf = new FileInfo(GetFile(model.Path));
                if (fileInf.Exists)
                {
                    fileInf.Delete();
                    // альтернатива с помощью класса File
                    // File.Delete(path);
                }
                StatusMessage = model.MessageDeleteOk();
                logger.LogInformation($"{model} edit");
                return RedirectToAction(nameof(Index), new { owner = model.ClaimId });
            }
            catch (Exception ex)
            {
                StatusMessage = $"{model.MessageDeleteError()} {ex.Message}.";
                return RedirectToAction(nameof(Index), new { owner = model.ClaimId });
            }
        }
        #endregion
        
        private string GetFile(string path)
        {
            return Path.Combine(hostingEnvironment.WebRootPath, FilesFolder, path);
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}
