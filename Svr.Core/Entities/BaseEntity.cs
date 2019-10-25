using System;
using System.ComponentModel.DataAnnotations;

namespace Svr.Core.Entities
{
    /// <summary>
    /// Базовый класс для сущности
    /// </summary>
    public abstract class BaseEntity
    {
        private const string Error = "Ошибка: ";
        protected const string ErrorStringEmpty = "Пожалуйста, заполните поле: {0}";
        protected const string ErrorStringMaxLength = "Максимальная длина поля: {0} не более {1} символов";
        /// <summary>
        /// Возвращает или задает идентификатор сущности
        /// </summary>
        [Key]
        public long Id { get; set; }
        /// <summary>
        /// Дата и время создания
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Дата создания")]
        public DateTime CreatedOnUtc { get; set; }
        /// <summary>
        /// Дата и время обновления
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Дата изменения")]
        public DateTime UpdatedOnUtc { get; set; }
        //[NotMapped]//чтобы не создавался столбец в таблице.
        public override string ToString() => "Базовая сущность";
        public virtual string MessageErrorFind() => $"{Error}{ToString()} с Id={Id}: Не удалось найти.";
        public virtual string MessageAddOk() => $"Добавлен: .";
        public virtual string MessageAddError() => $"{Error}{ToString()} - неудачная попытка регистрации.";
        public virtual string MessageEditOk() => $"Обновлен: {ToString()} с Id={Id}.";
        public virtual string MessageEditError() => $"{Error}{ToString()} с Id={Id}: Не удалось найти.";
        public virtual string MessageEditErrorNoknow() => $"{Error}{ToString()} с Id={Id}: Непредвиденная ошибка при обновлении.";
        public virtual string MessageDeleteOk() => $"Удален: {ToString()} с Id={Id}.";
        public virtual string MessageDeleteError() => $"{Error}{ToString()} с Id={Id}: Не удалось удалить.";
        // настройка каскадного удаления https://metanit.com/sharp/entityframeworkcore/3.2.php
    }
}
