using Microsoft.EntityFrameworkCore;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Core.Specifications;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Infrastructure.Data
{
    public class DirRepository : EfRepository<Dir>, IDirRepository
    {
        public DirRepository(DataContext context) : base(context)
        {
        }

        public IQueryable<Dir> Filter(string searchString = null, string lord = null, string owner = null, DateTime? dateS = null, DateTime? datePo = null, string category = null, string groupClaim = null, string subjectClaim = null, string resultClaim = null, bool? flgFilter = null)
        {
            var result = List(new DirSpecification(lord));
            return result;
        }

        public virtual Dir GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(d => d.DirName).Include(d => d.Applicants).AsNoTracking().FirstOrDefault(r => r.Id == id);
        }
        public virtual async Task<Dir> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await Entities.Include(r => r.DirName).Include(d => d.Applicants).AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        }
        public override IQueryable<Dir> Sort(IQueryable<Dir> source, SortState sortOrder)
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
                case SortState.OwnerAsc:
                    return source.OrderBy(s => s.DirName.Name);
                case SortState.OwnerDesc:
                    return source.OrderByDescending(s => s.DirName.Name);
                case SortState.NameAsc:
                    return source.OrderBy(s => s.Name);
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
                default:
                    return source.OrderBy(s => s.Name);
            }
        }
    }
}
