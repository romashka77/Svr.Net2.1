using System.Collections.Generic;

namespace Svr.Web.Models.CategoryDisputesViewModels
{
    public class IndexViewModel : StatusMessageViewModel
    {
        public IEnumerable<ItemViewModel> ItemViewModels { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
    }
}
