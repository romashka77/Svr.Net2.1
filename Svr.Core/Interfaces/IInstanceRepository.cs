using Svr.Core.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Core.Interfaces
{
    public interface IInstanceRepository : IRepository<Instance>, IRepositoryAsync<Instance>, ISort<Instance>
    {
        Instance GetByIdWithItems(long? id);
        Task<Instance> GetByIdWithItemsAsync(long? id);
        IQueryable<Instance> ListReport();
    }
}
