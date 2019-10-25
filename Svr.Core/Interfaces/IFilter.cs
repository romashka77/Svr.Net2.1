using Svr.Core.Entities;
using System;
using System.Linq;

namespace Svr.Core.Interfaces
{
    public interface IFilter<T> where T : BaseEntity
    {
        IQueryable<T> Filter(string searchString = null, string lord = null, string owner = null, DateTime? dateS = null,
            DateTime? datePo = null, string category = null, string groupClaim = null, string subjectClaim = null, string resultClaim = null, bool? flgFilter = null);
    }
}
