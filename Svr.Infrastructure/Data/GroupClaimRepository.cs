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
    public class GroupClaimRepository : EfRepository<GroupClaim>, IGroupClaimRepository
    {
        public GroupClaimRepository(DataContext context) : base(context)
        {
        }

        public IQueryable<GroupClaim> Filter(string searchString = null, string lord = null, string owner = null, DateTime? dateS = null, DateTime? datePo = null, string category = null, string groupClaim = null, string subjectClaim = null, string resultClaim = null, bool? flgFilter = null)
        {
            var result = List(new GroupClaimSpecification(category.ToLong())).OrderBy(a => String.Format("{0:d2}", a.Code.ToLong()));
            return result;
        }

        public virtual GroupClaim GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(c => c.SubjectClaims).Include(d => d.CategoryDispute).AsNoTracking().FirstOrDefault(r => r.Id == id);
        }
        public virtual async Task<GroupClaim> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await Entities.Include(r => r.CategoryDispute).Include(c => c.SubjectClaims).AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        }

        public override IQueryable<GroupClaim> Sort(IQueryable<GroupClaim> source, SortState sortOrder)
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
                    return source.OrderBy(s => s.CategoryDispute.Name);
                case SortState.OwnerDesc:
                    return source.OrderByDescending(s => s.CategoryDispute.Name);
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
