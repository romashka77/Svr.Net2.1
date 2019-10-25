using Microsoft.AspNetCore.Identity;
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
        public long? RegionId { get; set; }
        public long? DistrictId { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }//Фамилия

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }//Имя

        [MaxLength(100)]
        public string MiddleName { get; set; }//Отчество

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateofBirth { get; set; }//Дата рождения
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

    }
}
