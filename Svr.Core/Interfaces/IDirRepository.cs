using Svr.Core.Entities;
using System.Threading.Tasks;

namespace Svr.Core.Interfaces
{
    public interface IDirRepository : IRepository<Dir>, IRepositoryAsync<Dir>, ISort<Dir>, IFilter<Dir>
    {
        Dir GetByIdWithItems(long? id);
        Task<Dir> GetByIdWithItemsAsync(long? id);
    }
}
