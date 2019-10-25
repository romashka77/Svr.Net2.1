using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Svr.Core.Entities
{
    /// <inheritdoc />
    /// <summary>
    /// Иск
    /// </summary>
    public class Claim : BaseEntity
    {
        /// <summary>
        /// Регион
        /// </summary>
        public long RegionId { get; set; }
        [Display(Name = "Регион")]
        public virtual Region Region { get; set; }
        /// <summary>
        /// Район
        /// </summary>
        public long DistrictId { get; set; }
        [Display(Name = "Район")]
        public virtual District District { get; set; }
        /// <summary>
        /// Рег.номер
        /// </summary>
        [MaxLength(100, ErrorMessage = ErrorStringMaxLength)]
        [Display(Name = "Рег.номер", Prompt = "Введите регистрационный номер")]
        public string Code { get; set; }
        /// <summary>
        /// № дела
        /// </summary>
        [MaxLength(50, ErrorMessage = ErrorStringMaxLength)]
        [Display(Name = "№ дела", Prompt = "Введите № дела")]
        [Required(ErrorMessage = ErrorStringEmpty)]
        public string Name { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        [Display(Name = "Описание", Prompt = "Введите описание")]
        public string Description { get; set; }
        /// <summary>
        /// Дата регистрации
        /// </summary>
        [Display(Name = "Дата регистрации")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = ErrorStringEmpty)]
        public DateTime DateReg { get; set; }
        /// <summary>
        /// Дата принятия иска
        /// </summary>
        [Display(Name = "Дата принятия иска")]
        [DataType(DataType.Date)]
        public DateTime? DateIn { get; set; }
        /// <summary>
        /// Id Категории споров 
        /// </summary>
        [Display(Name = "Категория споров")]
        public long? CategoryDisputeId { get; set; }
        /// <summary>
        /// Категория споров
        /// </summary>
        [Display(Name = "Категория споров")]
        public virtual CategoryDispute CategoryDispute { get; set; }
        /// <summary>
        /// Id Группы исков
        /// </summary>
        [Display(Name = "Группа исков")]
        public long? GroupClaimId { get; set; }
        /// <summary>
        /// Группа исков
        /// </summary>
        [Display(Name = "Группа исков")]
        public virtual GroupClaim GroupClaim { get; set; }
        /// <summary>
        /// Id Предмета иска
        /// </summary>
        [Display(Name = "Предмет иска")]
        public long? SubjectClaimId { get; set; }
        /// <summary>
        /// Предмет иска
        /// </summary>
        [Display(Name = "Предмет иска")]
        public virtual SubjectClaim SubjectClaim { get; set; }
        /// <summary>
        /// Id Суда
        /// </summary>
        public long? СourtId { get; set; }
        /// <summary>
        /// Суд
        /// </summary>
        [Display(Name = "Суд")]
        public virtual Dir Сourt { get; set; }
        /// <summary>
        /// Id Исполнителя
        /// </summary>
        public long? PerformerId { get; set; }
        /// <summary>
        /// Исполнитель
        /// </summary>
        [Display(Name = "Исполнитель")]
        public virtual Performer Performer { get; set; }
        /// <summary>
        /// Сумма иска
        /// </summary>
        [Display(Name = "Сумма иска")]
        [Column(TypeName = "money")]
        public decimal? Sum { get; set; }
        /// <summary>
        /// Id Истца
        /// </summary>
        public long? PlaintiffId { get; set; }
        /// <summary>
        /// Истец
        /// </summary>
        [Display(Name = "Истец")]
        [ForeignKey("PlaintiffId")]
        public virtual Applicant Plaintiff { get; set; }
        /// <summary>
        /// Id Ответчика
        /// </summary>
        public long? RespondentId { get; set; }
        /// <summary>
        /// Ответчик
        /// </summary>
        [Display(Name = "Ответчик")]
        [ForeignKey("RespondentId")]
        public virtual Applicant Respondent { get; set; }

        /// <summary>
        /// Id 3-его лица
        /// </summary>
        public long? Person3rdId { get; set; }
        /// <summary>
        /// 3-е лицо
        /// </summary>
        [Display(Name = "3-е лицо")]
        [ForeignKey("Person3rdId")]
        public virtual Applicant Person3rd { get; set; }
        /// <summary>
        /// Дата вступления в законную силу
        /// </summary>
        [Display(Name = "Дата вступления в законную силу")]
        [DataType(DataType.Date)]
        public DateTime? DateForce { get; set; }
        /// <summary>
        /// Итоговое решение суда
        /// </summary>
        [Display(Name = "Итоговое решение суда")]
        public string FinalDecision { get; set; }
        /// <summary>
        /// Коллекция инстанций
        /// </summary>
        [Display(Name = "Инстанции")]
        public virtual ICollection<Instance> Instances { get; set; } = new List<Instance>();
        /// <summary>
        /// Коллекция заседаний
        /// </summary>
        [Display(Name = "График заседаний")]
        public virtual ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();
        /// <summary>
        /// Коллекция файлов
        /// </summary>
        [Display(Name = "Документы по иску")]
        public virtual ICollection<FileEntity> FileEntities { get; set; } = new List<FileEntity>();

        public override string ToString() => "Иск";
    }
}
