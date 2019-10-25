using Svr.Core.Entities;
using Svr.Core.Interfaces;

namespace Svr.Infrastructure.Data
{
    public class ManRepository : EfRepository<Man>, IManRepository
    {
        public ManRepository(DataContext context) : base(context)
        {

        }
    }
}
