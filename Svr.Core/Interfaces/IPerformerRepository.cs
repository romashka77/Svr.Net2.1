using Svr.Core.Entities;
using System.Threading.Tasks;

namespace Svr.Core.Interfaces
{
    public interface IPerformerRepository : IRepository<Performer>, IRepositoryAsync<Performer>, ISort<Performer>
    {
        Performer GetByIdWithItems(long? id);
        Task<Performer> GetByIdWithItemsAsync(long? id);
    }
}
