using Svr.Infrastructure.Identity;

namespace Svr.Web.Models.UsersViewModels
{
    public class ItemViewModel : ApplicationUser
    {
        public string StatusMessage { get; set; }
    }
}
