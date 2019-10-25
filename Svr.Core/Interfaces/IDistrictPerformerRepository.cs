using Svr.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Svr.Core.Interfaces
{
    public interface IDistrictPerformerRepository
    {
        IEnumerable<DistrictPerformer> List(ISpecification<DistrictPerformer> spec);
        Task<List<DistrictPerformer>> ListAsync(ISpecification<DistrictPerformer> spec);
        Task<DistrictPerformer> AddAsync(DistrictPerformer entity);
        void Delete(DistrictPerformer entity);
        Task ClearAsync(ISpecification<DistrictPerformer> spec);
        Task DeleteAsync(DistrictPerformer entity);
    }
}
