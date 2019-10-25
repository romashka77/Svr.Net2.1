using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    /// <inheritdoc />
    /// <summary>
    /// Базовая сущность с наименованием и описанием
    /// </summary>
    public abstract class BaseEntityDescription : BaseEntityName
    {
        /// <summary>
        /// Описание
        /// </summary>
        [Display(Name = "Описание", Prompt = "Введите описание")]
        public string Description { get; set; }
        public override string ToString() => "Базовая сущность с наименованием и описанием";
    }
}
