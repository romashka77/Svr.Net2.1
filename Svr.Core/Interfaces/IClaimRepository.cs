using Svr.Core.Entities;
using System.Threading.Tasks;

namespace Svr.Core.Interfaces
{
    public interface IClaimRepository : IRepository<Claim>, IRepositoryAsync<Claim>, ISort<Claim>, IFilter<Claim>
    {
        Claim GetByIdWithItems(long? id);
        Task<Claim> GetByIdWithItemsAsync(long? id);
    }
}
