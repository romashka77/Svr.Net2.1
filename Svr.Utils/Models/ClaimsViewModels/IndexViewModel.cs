﻿using Svr.Utils.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Svr.Utils.Models.ClaimsViewModels
{
    public class IndexViewModel : StatusMessageViewModel
    {
        public IEnumerable<ItemViewModel> ItemViewModels { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
    }
}
