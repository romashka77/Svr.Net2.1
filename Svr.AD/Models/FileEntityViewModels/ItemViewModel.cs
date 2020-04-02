using Microsoft.AspNetCore.Http;
using Svr.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Svr.AD.Models.FileEntityViewModels
{
    public class ItemViewModel : FileEntity
    {
        [Display(Name = "Выберите файл для загрузки")]
        public IFormFile UploadedFile { get; set; }
        public string StatusMessage { get; set; }
    }
}
