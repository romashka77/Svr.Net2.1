using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Svr.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Svr.Web.Models.ClaimsViewModels
{
    public class CreateViewModel : BaseEntity, IValidatableObject
    {
        [MaxLength(50, ErrorMessage = ErrorStringMaxLength)]
        [Display(Name = "№ дела", Prompt = "Введите № дела")]
        [Required(ErrorMessage = ErrorStringEmpty)]
        public string Name { get; set; }
        [Display(Name = "Описание", Prompt = "Введите описание")]
        public string Description { get; set; }
        [Display(Name = "Регион")]
        [Required(ErrorMessage = ErrorStringEmpty)]
        public long RegionId { get; set; }
        public IEnumerable<SelectListItem> Regions { get; set; } // список владельцев владельцев
        [Display(Name = "Район")]
        [Required(ErrorMessage = ErrorStringEmpty)]
        public long DistrictId { get; set; }
        public IEnumerable<SelectListItem> Districts { get; set; } // список владельцев

        [Display(Name = "Дата регистрации")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = ErrorStringEmpty)]
        [Remote("ValidateDate", "Claims")]
        public DateTime DateReg { get; set; }
        public override string ToString() => "Иск";

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateReg.Year < 2018)
            {
                yield return new ValidationResult(
                    $"Год должен быть > 2017",
                    new[] { nameof(DateReg) });
            }
        }
    }
}
