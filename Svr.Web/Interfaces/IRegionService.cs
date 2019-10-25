using Svr.Core.Entities;
using Svr.Core.Interfaces;
using Svr.Web.Models.RegionsViewModels;
using System.Threading.Tasks;

namespace Svr.Web.Interfaces
{
    public interface IRegionService : IRegionRepository
    {
        Task<IndexViewModel> GetRegionItems(SortState sortOrder = SortState.NameAsc, string name = null, int pageIndex = 1, int itemsPage = 10);

    }
}
