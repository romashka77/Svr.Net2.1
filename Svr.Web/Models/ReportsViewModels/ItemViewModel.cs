using System;
using System.ComponentModel.DataAnnotations;

namespace Svr.Web.Models.ReportsViewModels
{
    public class ItemViewModel //: Report
    {
        [Display(Name = "Имя файла")]
        public string Name { get; set; }
        [Display(Name = "Расширение")]
        public string Code { get; set; }
        //[DataType(DataType.Date)]
        [Display(Name = "Дата создания")]
        public DateTime CreatedOnUtc { get; set; }
        //[DataType(DataType.Date)]
        [Display(Name = "Дата изменения")]
        public DateTime UpdatedOnUtc { get; set; }
        public string StatusMessage { get; set; }
        public override string ToString() => "Отчет";
    }
}
