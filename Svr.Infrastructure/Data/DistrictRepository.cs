using Microsoft.EntityFrameworkCore;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using Svr.Infrastructure.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Infrastructure.Data
{
    public class DistrictRepository : EfRepository<District>, IDistrictRepository
    {
        public DistrictRepository(DataContext context) : base(context)
        {
        }
        public virtual District GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(d => d.Region).Include(d => d.DistrictPerformers).ThenInclude(e => e.Performer).AsNoTracking().SingleOrDefault(m => m.Id == id);
        }
        public virtual async Task<District> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await Entities.Include(d => d.Region).Include(d => d.DistrictPerformers).ThenInclude(e => e.Performer).AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);
        }
        public override IQueryable<District> Sort(IQueryable<District> source, SortState sortOrder)
        {
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    return source.OrderByDescending(p => p.Name);
                case SortState.CodeAsc:
                    return source.OrderBy(p => p.Code);
                case SortState.CodeDesc:
                    return source.OrderByDescending(p => p.Code);
                case SortState.DescriptionAsc:
                    return source.OrderBy(p => p.Description);
                case SortState.DescriptionDesc:
                    return source.OrderByDescending(p => p.Description);
                case SortState.CreatedOnUtcAsc:
                    return source.OrderBy(p => p.CreatedOnUtc);
                case SortState.CreatedOnUtcDesc:
                    return source.OrderByDescending(p => p.CreatedOnUtc);
                case SortState.UpdatedOnUtcAsc:
                    return source.OrderBy(p => p.UpdatedOnUtc);
                case SortState.UpdatedOnUtcDesc:
                    return source.OrderByDescending(p => p.UpdatedOnUtc);
                case SortState.OwnerAsc:
                    return source.OrderBy(s => s.Region.Name);
                case SortState.OwnerDesc:
                    return source.OrderByDescending(s => s.Region.Name);
                case SortState.NameAsc:
                    return source.OrderBy(s => s.Name);
                case SortState.LordAsc:
                    return source;
                case SortState.LordDesc:
                    return source;
                default:
                    return source.OrderBy(s => s.Name);
            }
        }

        public IQueryable<District> Filter(string searchString = null, string lord = null, string owner = null, DateTime? dateS = null, DateTime? datePo = null, string category = null, string groupClaim = null, string subjectClaim = null, string resultClaim = null, bool? flgFilter = null)
        {
            var result = List(new DistrictSpecification(lord.ToLong()));
            if (!String.IsNullOrEmpty(searchString))
            {
                result = result.Where(d => d.Name.ToUpper().Contains(searchString.ToUpper()) || d.Code.ToUpper().Contains(searchString.ToUpper()));
            }
            if ((bool)flgFilter)
                result = result.Where(n => n.Id.ToString() == owner);
            return result;
        }
    }
}
