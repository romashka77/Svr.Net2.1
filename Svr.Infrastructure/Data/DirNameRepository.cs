using Microsoft.EntityFrameworkCore;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Infrastructure.Data
{
    public class DirNameRepository : EfRepository<DirName>, IDirNameRepository
    {
        public DirNameRepository(DataContext dbContext) : base(dbContext)
        {

        }

        public virtual DirName GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(r => r.Dirs).AsNoTracking().FirstOrDefault(r => r.Id == id);
        }

        public virtual async Task<DirName> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await Entities.Include(r => r.Dirs).AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        }
        public override IQueryable<DirName> Sort(IQueryable<DirName> source, SortState sortOrder)
        {
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    return source.OrderByDescending(p => p.Name);
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
                case SortState.CodeAsc:
                    return source;
                case SortState.CodeDesc:
                    return source;
                case SortState.DescriptionAsc:
                    return source;
                case SortState.DescriptionDesc:
                    return source;
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
