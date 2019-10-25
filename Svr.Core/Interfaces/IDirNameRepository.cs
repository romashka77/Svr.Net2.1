using Svr.Core.Entities;
using System.Threading.Tasks;

namespace Svr.Core.Interfaces
{
    public interface IDirNameRepository : IRepository<DirName>, IRepositoryAsync<DirName>, ISort<DirName>
    {
        DirName GetByIdWithItems(long? id);
        Task<DirName> GetByIdWithItemsAsync(long? id);
    }
}
