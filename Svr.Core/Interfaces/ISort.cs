using Svr.Core.Entities;
using System.Linq;

namespace Svr.Core.Interfaces
{
    public interface ISort<T> where T : BaseEntity
    {
        IQueryable<T> Sort(IQueryable<T> source, SortState sortOrder);
    }
}
