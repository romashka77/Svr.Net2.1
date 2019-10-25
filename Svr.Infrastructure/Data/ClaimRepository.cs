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
    public class ClaimRepository : EfRepository<Claim>, IClaimRepository
    {
        public ClaimRepository(DataContext context) : base(context)
        {

        }

        public virtual Claim GetByIdWithItems(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Entities.Include(f => f.FileEntities).Include(m => m.Meetings).Include(i => i.Instances).Include(d => d.District).Include(e => e.Region).AsNoTracking().SingleOrDefault(m => m.Id == id);
        }

        public virtual async Task<Claim> GetByIdWithItemsAsync(long? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return await Entities.Include(f => f.FileEntities).Include(m => m.Meetings).Include(i => i.Instances).Include(d => d.District).Include(e => e.Region).AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);
        }
        public IQueryable<Claim> Filter(string searchString = null, string lord = null, string owner = null, DateTime? dateS = null, DateTime? datePo = null, string category = null, string groupClaim = null, string subjectClaim = null, string resultClaim = null, bool? flgFilter = null)
        {
            var result = List(new ClaimSpecification(owner.ToLong()));
            if (!string.IsNullOrWhiteSpace(subjectClaim))
            {
                result = result.Where(c => c.SubjectClaimId.ToString() == subjectClaim);
            }
            else if (!string.IsNullOrWhiteSpace(groupClaim))
            {
                result = result.Where(c => c.GroupClaimId.ToString() == groupClaim);
            }
            else if (!string.IsNullOrWhiteSpace(category))
                result = result.Where(c => c.CategoryDisputeId.ToString() == category);
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                result = result.Where(d => d.Name.ToUpper()
                                                 .Contains(searchString.ToUpper()) || d.Code.ToUpper().Contains(searchString.ToUpper()) || d.SubjectClaim.Code.ToUpper().Contains(searchString.ToUpper()));
            }
            if (dateS != null)
                result = result.Where(c => c.DateIn >= dateS);
            if (datePo != null)
                result = result.Where(c => c.DateIn <= datePo);
            if (!string.IsNullOrWhiteSpace(resultClaim))
            {
                result = result.Include(a => a.Instances).ThenInclude(i => i.CourtDecision);
                result = result.Where(a => a.Instances.Where(b => b.CourtDecision.Name == resultClaim).Count() > 0);
                //list = list.Where(a => !string.IsNullOrWhiteSpace(a.Instances.Last().ToString()) && a.Instances.Last().CourtDecision.Name == resultClaim);
            }
            return result;
        }
        public override IQueryable<Claim> Sort(IQueryable<Claim> source, SortState sortOrder)
        {
            switch (sortOrder)
            {
                case SortState.NameDesc:
                    return source.OrderByDescending(p => p.Name);
                case SortState.CodeAsc:
                    return source.OrderBy(p => p.Code);
                case SortState.CodeDesc:
                    return source.OrderByDescending(p => p.Code);
                case SortState.SumAsc:
                    return source.OrderBy(p => p.Sum);
                case SortState.SumDesc:
                    return source.OrderByDescending(p => p.Sum);
                case SortState.DescriptionAsc:
                    return source.OrderBy(p => p.Description);
                case SortState.DescriptionDesc:
                    return source.OrderByDescending(p => p.Description);
                case SortState.CreatedOnUtcAsc://Дата принятия иска по возрастанию
                    return source.OrderBy(s => s.DateIn);
                case SortState.CreatedOnUtcDesc:
                    return source.OrderByDescending(s => s.DateIn);
                case SortState.CodeSubjectClaimAsc:
                    return source.OrderBy(p => p.SubjectClaim.Code);
                case SortState.CodeSubjectClaimDesc:
                    return source.OrderByDescending(p => p.SubjectClaim.Code);
                case SortState.OwnerAsc:
                    return source.OrderBy(s => s.District.Name);
                case SortState.OwnerDesc:
                    return source.OrderByDescending(s => s.District.Name);
                case SortState.NameAsc:
                    return source.OrderBy(s => s.Name);
                case SortState.LordAsc:
                    return source.OrderBy(s => s.Region.Name);
                case SortState.LordDesc:
                    return source.OrderByDescending(s => s.Region.Name);
                default:
                    return source.OrderBy(s => s.Name);
            }
        }

    }
}
