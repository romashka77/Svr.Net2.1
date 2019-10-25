using Svr.Core.Entities;
using System.Threading.Tasks;

namespace Svr.Core.Interfaces
{
    public interface ICategoryDisputeRepository : IRepository<CategoryDispute>, IRepositoryAsync<CategoryDispute>, ISort<CategoryDispute>, IFilter<CategoryDispute>
    {
        CategoryDispute GetByIdWithItems(long? id);
        Task<CategoryDispute> GetByIdWithItemsAsync(long? id);
    }
}
