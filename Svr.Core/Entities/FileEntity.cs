using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    /// <summary>
    /// Файл
    /// </summary>
    public class FileEntity : BaseEntity
    {
        /// <summary>
        /// Id Иска
        /// </summary>
        public long ClaimId { get; set; }
        /// <summary>
        /// Иск
        /// </summary>
        [Display(Name = "Иск")]
        public virtual Claim Claim { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        [Display(Name = "Наименование", Prompt = "Введите наименование")]
        [MaxLength(100, ErrorMessage = ErrorStringMaxLength)]
        public string Name { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        [Display(Name = "Описание", Prompt = "Введите описание")]
        public string Description { get; set; }
        /// <summary>
        /// Имя файла
        /// </summary>
        [Display(Name = "Имя файла")]
        public string Path { get; set; }

        public override string ToString() => "Файл";
    }
}
