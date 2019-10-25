using Microsoft.EntityFrameworkCore;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Infrastructure.Data
{
    public class RegionRepository : EfRepository<Region>, IRegionRepository
    {
        public RegionRepository(DataContext dbContext) : base(dbContext)
        {

        }
        public IQueryable<Region> Filter(string searchString = null, string lord = null, string owner = null, DateTime? dateS = null, DateTime? datePo = null, string category = null, string groupClaim = null, string subjectClaim = null, string resultClaim = null, bool? flgFilter = null)
        {
            var result = ListAll();
            if ((bool)flgFilter)
                result = result.Where(n => n.Id.ToString() == lord);
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                result = result.Where(p => p.Name.ToUpper().Contains(searchString.ToUpper()) || p.Code.ToUpper().Contains(searchString.ToUpper()));
            }
            return result;
        }
        public virtual Region GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(r => r.Districts).Include(p => p.Performers).AsNoTracking().FirstOrDefault(r => r.Id == id);
        }

        public virtual async Task<Region> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await Entities.Include(r => r.Districts).Include(p => p.Performers).AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        }


        public override IQueryable<Region> Sort(IQueryable<Region> source, SortState sortOrder)
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
                case SortState.NameAsc:
                    return source.OrderBy(p => p.Name);
                case SortState.LordAsc:
                    return source;
                case SortState.LordDesc:
                    return source;
                case SortState.OwnerAsc:
                    return source;
                case SortState.OwnerDesc:
                    return source;
                default:
                    return source.OrderBy(p => p.Name);
            }
        }
    }
}
