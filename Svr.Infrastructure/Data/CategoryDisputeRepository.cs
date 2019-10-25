using Microsoft.EntityFrameworkCore;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace Svr.Infrastructure.Data
{
    public class CategoryDisputeRepository : EfRepository<CategoryDispute>, ICategoryDisputeRepository
    {
        public CategoryDisputeRepository(DataContext dbContext) : base(dbContext)
        {

        }

        public IQueryable<CategoryDispute> Filter(string searchString = null, string lord = null, string owner = null, DateTime? dateS = null, DateTime? datePo = null, string category = null, string groupClaim = null, string subjectClaim = null, string resultClaim = null, bool? flgFilter = null)
        {
            return ListAll();
        }

        public virtual CategoryDispute GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(c => c.GroupClaims).AsNoTracking().FirstOrDefault(r => r.Id == id);
        }
        public virtual async Task<CategoryDispute> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await Entities.Include(c => c.GroupClaims).AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        }
        public override IQueryable<CategoryDispute> Sort(IQueryable<CategoryDispute> source, SortState sortOrder)
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
                case SortState.NameAsc:
                    return source.OrderBy(p => p.Name);
                case SortState.CodeAsc:
                    return source;
                case SortState.CodeDesc:
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
