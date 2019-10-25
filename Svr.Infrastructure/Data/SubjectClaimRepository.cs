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
    public class SubjectClaimRepository : EfRepository<SubjectClaim>, ISubjectClaimRepository
    {
        public SubjectClaimRepository(DataContext context) : base(context)
        {
        }

        public IQueryable<SubjectClaim> Filter(string searchString = null, string lord = null, string owner = null, DateTime? dateS = null, DateTime? datePo = null, string category = null, string groupClaim = null, string subjectClaim = null, string resultClaim = null, bool? flgFilter = null)
        {
            return List(new SubjectClaimSpecification(groupClaim.ToLong())).OrderBy(a => String.Format("{0:d2}", a.GroupClaim.Code.ToLong()));
        }

        public virtual SubjectClaim GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(d => d.GroupClaim).AsNoTracking().FirstOrDefault(r => r.Id == id);
        }
        public virtual async Task<SubjectClaim> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await Entities.Include(r => r.GroupClaim).AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        }
        public override IQueryable<SubjectClaim> Sort(IQueryable<SubjectClaim> source, SortState sortOrder)
        {
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    return source.OrderByDescending(p => p.Name);
                case SortState.CodeAsc:
                    return source.OrderBy(p => p.Code/*, codeComparer*/);
                case SortState.CodeDesc:
                    return source.OrderByDescending(p => p.Code/*, codeComparer*/);
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
                    return source.OrderBy(s => s.GroupClaim.Name);
                case SortState.OwnerDesc:
                    return source.OrderByDescending(s => s.GroupClaim.Name);
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
    }
}
