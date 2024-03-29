﻿using Svr.Core.Entities;
using Svr.Utils.Models;

using System.Collections.Generic;

namespace Svr.Web.Models.MeetingsViewModels
{
    public class IndexViewModel : StatusMessageViewModel
    {
        public Claim Claim { get; set; }
        //public long? Owner { get; set; }
        public IEnumerable<ItemViewModel> ItemViewModels { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
    }
}
