using Microsoft.AspNetCore.Identity;
using Svr.Core.Entities;

using System;
using System.ComponentModel.DataAnnotations;

namespace Svr.Infrastructure.Identity
{
    //Добавление данных профиля для пользователей приложения путем добавления свойств в класс ApplicationUser

    //    В ASP.NET Core Identity пользователь представлен классом IdentityUser из пространства имен Microsoft.AspNetCore.Identity.EntityFrameworkCore.Этот класс предоставляет базовую информацию о пользователе с помощью следующих свойств:

    //Id: уникальный идентификатор пользователя
    //UserName: ник пользователя
    //Email: электронный адрес пользователя
    //Logins: коллекция логинов, которые использовались пользователем для входа через сторонние сервисы (Google, Facebook и т.д.)
    //Claims: коллекция клеймов или дополнительных объектов, которые используются для авторизации пользователя
    //PasswordHash: хеш пароля.В базе данных напрямую не хранится пароль, а только его хеш.
    //Roles: набор ролей, к которым принадлежит пользователь
    //PhoneNumber: номер телефона
    //SecurityStamp: некоторое специальное значение, которое меняется при смене аутентификационных данных, например, пароля
    //AccessFailedCount: количество неудачных входов пользователя в систему
    //EmailConfirmed: подтвержден ли адрес электронной почты
    //PhoneNumberConfirmed: подтвержден ли номер телефона
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Регион
        /// </summary>
        public long? RegionId { get; set; }
        //[Display(Name = "Регион")]
        //public virtual Region Region { get; set; }
        /// <summary>
        /// Район
        /// </summary>
        public long? DistrictId { get; set; }
        //[Display(Name = "Район")]
        //public virtual District District { get; set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
        /// <summary>
        /// Отчество
        /// </summary>
        [MaxLength(100)]
        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }
        /// <summary>
        /// Дата рождения
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Отчество")]
        public DateTime DateofBirth { get; set; }
        /// <summary>
        /// Дата и время создания
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime CreatedOnUtc { get; set; }
        /// <summary>
        /// Дата и время обновления
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime UpdatedOnUtc { get; set; }

        public virtual string ToString() => "Пользователь";

    }
}
