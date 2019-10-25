using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Svr.Web.Models
{
    public class FilterViewModel
    {
        #region конструктор
        public FilterViewModel(string searchString, string owner = null, IEnumerable<SelectListItem> owners = null, string lord = null, IEnumerable<SelectListItem> lords = null, DateTime? dateS = null, DateTime? datePo = null, string category = null, IEnumerable<SelectListItem> categores = null, string groupClaim = null, IEnumerable<SelectListItem> groupClaims = null, string subjectClaim = null, IEnumerable<SelectListItem> subjectClaims = null, string resultClaim = null, IEnumerable<SelectListItem> resultClaims = null, long? itemsCount = 0)
        {
            // устанавливаем начальный элемент, который позволит выбрать всех
            //owners.Insert(new  { Name = "Все", Id = 0 });
            //Owners = owners.Select(a => new SelectListItem { Text=a.}); new SelectList(owners, "Id", "Name", owner);
            Lords = lords;
            Owners = owners;
            Сategores = categores;
            GroupClaims = groupClaims;
            SubjectClaims = subjectClaims;
            ResultClaims = resultClaims;
            SelectedLord = lord;
            SelectedOwner = owner;
            SelectedCategory = category;
            SelectedGroupClaim = groupClaim;
            SelectedSubjectClaim = subjectClaim;
            SelectedResultClaim = resultClaim;
            SearchString = searchString;
            DateS = dateS;
            DatePo = datePo;
            ItemsCount = itemsCount;
        }
        #endregion
        public IEnumerable<SelectListItem> Lords { get; private set; } // список владельцев владельцев
        public IEnumerable<SelectListItem> Owners { get; private set; } // список владельцев
        public IEnumerable<SelectListItem> Сategores { get; private set; } // список категорий
        public IEnumerable<SelectListItem> GroupClaims { get; private set; } // список групп исков
        public IEnumerable<SelectListItem> SubjectClaims { get; private set; } // список предметов исков
        public IEnumerable<SelectListItem> ResultClaims { get; private set; } // список результатов рассмотрения

        public string SelectedLord { get; set; }   // выбранный владелец владельцев
        public string SelectedOwner { get; set; }   // выбранный владелец
        public string SelectedCategory { get; set; }   // выбранная категория
        public string SelectedGroupClaim { get; set; }   // выбранная группа исков
        public string SelectedSubjectClaim { get; set; }   // выбранный предмет иска
        public string SelectedResultClaim { get; set; }   // выбранный результат рассмотрения
        [Display(Name = "Поиск:")]
        public string SearchString { get; private set; }    // строка поиска
        [DataType(DataType.Date)]
        [Display(Name = "Дата с")]
        public DateTime? DateS { get; private set; }
        [DataType(DataType.Date)]
        [Display(Name = "Дата по")]
        public DateTime? DatePo { get; private set; }
        [Display(Name = "Кол-во")]
        public long? ItemsCount { get; private set; }
    }
}
