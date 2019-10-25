using Svr.Core.Entities;
using System.Threading.Tasks;

namespace Svr.Core.Interfaces
{
    public interface IDistrictRepository : IRepository<District>, IRepositoryAsync<District>, ISort<District>, IFilter<District>
    {
        District GetByIdWithItems(long? id);
        Task<District> GetByIdWithItemsAsync(long? id);
    }
}
