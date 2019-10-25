using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    /// <inheritdoc />
    /// <summary>
    /// Базовая сущность c наименованием
    /// </summary>
    public abstract class BaseEntityName : BaseEntity
    {
        /// <summary>
        /// Наименование
        /// </summary>
        [Required(ErrorMessage = ErrorStringEmpty)]
        [Display(Name = "Наименование", Prompt = "Введите наименование")]
        [MaxLength(100, ErrorMessage = ErrorStringMaxLength)]
        public string Name { get; set; }
        public override string ToString() => "Базовая сущность c наименованием";
    }
}
