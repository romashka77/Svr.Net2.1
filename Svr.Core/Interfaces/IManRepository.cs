using Svr.Core.Entities;

namespace Svr.Core.Interfaces
{
    public interface IManRepository : IRepository<Man>, IRepositoryAsync<Man>
    {
    }
}
