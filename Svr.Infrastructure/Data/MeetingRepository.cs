using Microsoft.EntityFrameworkCore;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Infrastructure.Data
{
    public class MeetingRepository : EfRepository<Meeting>, IMeetingRepository
    {
        public MeetingRepository(DataContext context) : base(context)
        {
        }
        public virtual Meeting GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(d => d.Claim).AsNoTracking().SingleOrDefault(r => r.Id == id);
        }
        public virtual async Task<Meeting> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await Entities.Include(d => d.Claim).AsNoTracking().SingleOrDefaultAsync(r => r.Id == id);
        }
        public override IQueryable<Meeting> Sort(IQueryable<Meeting> source, SortState sortOrder)
        {
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    return source.OrderByDescending(p => p.Name);
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
                    return source.OrderBy(s => s.Claim.Name);
                case SortState.OwnerDesc:
                    return source.OrderByDescending(s => s.Claim.Name);
                case SortState.NameAsc:
                    return source.OrderBy(s => s.Name);
                case SortState.CodeAsc:
                    return source;
                case SortState.CodeDesc:
                    return source;
                case SortState.LordAsc:
                    return source;
                case SortState.LordDesc:
                    return source;
                default:
                    return source.OrderBy(s => s.Name);
            }
        }
    }
}
