using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Svr.Core.Entities;
using System;

namespace Svr.Web.Models
{
    public class SortHeaderTagHelper : TagHelper
    {
        //public string CurrentFilterName { get; set; }// фильтрация
        public string SearchString { get; set; }// фильтрация
        public string CurrentFilterOwner { get; set; }
        public string CurrentFilterLord { get; set; }
        public string CurrentFilterCategory { get; set; }
        public string CurrentFilterGroupClaim { get; set; }
        public string CurrentFilterSubjectClaim { get; set; }
        public string CurrentFilterResultClaim { get; set; }
        public DateTime? CurrentFilterDateS { get; set; }
        public DateTime? CurrentFilterDatePo { get; set; }

        public SortState Property { get; set; } // значение текущего свойства, для которого создается тег
        public SortState Current { get; set; }  // значение активного свойства, выбранного для сортировки
        public string Action { get; set; }  // действие контроллера, на которое создается ссылка
        public bool Up { get; set; }    // сортировка по возрастанию или убыванию

        private IUrlHelperFactory urlHelperFactory;
        #region конструктор
        public SortHeaderTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }
        #endregion
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "a";
            string url = urlHelper.Action(Action, new
            {
                sortOrder = Property,
                searchString = SearchString,
                owner = CurrentFilterOwner,
                lord = CurrentFilterLord,
                dateS = CurrentFilterDateS,
                datePo = CurrentFilterDatePo,
                category = CurrentFilterCategory,
                groupClaim = CurrentFilterGroupClaim,
                subjectClaim = CurrentFilterSubjectClaim,
                resultClaim = CurrentFilterResultClaim
            });
            output.Attributes.SetAttribute("href", url);
            // если текущее свойство имеет значение CurrentSort
            if (((Current == SortState.CodeAsc) && (Property == SortState.CodeDesc)) || ((Current == SortState.CodeDesc) && (Property == SortState.CodeAsc)) ||
                ((Current == SortState.NameAsc) && (Property == SortState.NameDesc)) || ((Current == SortState.NameDesc) && (Property == SortState.NameAsc)) ||
                ((Current == SortState.DescriptionAsc) && (Property == SortState.DescriptionDesc)) || ((Current == SortState.DescriptionDesc) && (Property == SortState.DescriptionAsc)) ||
                ((Current == SortState.CreatedOnUtcAsc) && (Property == SortState.CreatedOnUtcDesc)) || ((Current == SortState.CreatedOnUtcDesc) && (Property == SortState.CreatedOnUtcAsc)) ||
                ((Current == SortState.UpdatedOnUtcAsc) && (Property == SortState.UpdatedOnUtcDesc)) || ((Current == SortState.UpdatedOnUtcDesc) && (Property == SortState.UpdatedOnUtcAsc)) ||
                ((Current == SortState.OwnerAsc) && (Property == SortState.OwnerDesc)) || ((Current == SortState.OwnerDesc) && (Property == SortState.OwnerAsc)) ||
                ((Current == SortState.LordAsc) && (Property == SortState.LordDesc)) || ((Current == SortState.LordDesc) && (Property == SortState.LordAsc)) ||
                ((Current == SortState.CodeSubjectClaimAsc) && (Property == SortState.CodeSubjectClaimDesc)) || ((Current == SortState.CodeSubjectClaimDesc) && (Property == SortState.CodeSubjectClaimAsc)) ||
                ((Current == SortState.SumAsc) && (Property == SortState.SumDesc)) || ((Current == SortState.SumDesc) && (Property == SortState.SumAsc))
                )
            {
                TagBuilder tag = new TagBuilder("i");
                tag.AddCssClass("glyphicon");

                if (Up == true)   // если сортировка по возрастанию
                    tag.AddCssClass("glyphicon-chevron-up");
                else   // если сортировка по убыванию
                    tag.AddCssClass("glyphicon-chevron-down");

                output.PreContent.AppendHtml(tag);
            }
        }
    }
}
