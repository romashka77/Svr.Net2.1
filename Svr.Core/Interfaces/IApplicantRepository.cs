using Svr.Core.Entities;
using System.Threading.Tasks;

namespace Svr.Core.Interfaces
{
    public interface IApplicantRepository : IRepository<Applicant>, IRepositoryAsync<Applicant>, ISort<Applicant>, IFilter<Applicant>
    {
        Applicant GetByIdWithItems(long? id);
        Task<Applicant> GetByIdWithItemsAsync(long? id);
    }
}
