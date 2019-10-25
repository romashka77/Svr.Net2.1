using Svr.Core.Entities;
using System.Threading.Tasks;

namespace Svr.Core.Interfaces
{
    public interface IRegionRepository : IRepository<Region>, IRepositoryAsync<Region>, ISort<Region>, IFilter<Region>
    {
        Region GetByIdWithItems(long? id);
        Task<Region> GetByIdWithItemsAsync(long? id);
    }
}
