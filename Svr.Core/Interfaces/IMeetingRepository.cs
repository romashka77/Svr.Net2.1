using Svr.Core.Entities;
using System.Threading.Tasks;

namespace Svr.Core.Interfaces
{
    public interface IMeetingRepository : IRepository<Meeting>, IRepositoryAsync<Meeting>, ISort<Meeting>
    {
        Meeting GetByIdWithItems(long? id);
        Task<Meeting> GetByIdWithItemsAsync(long? id);
    }
}
