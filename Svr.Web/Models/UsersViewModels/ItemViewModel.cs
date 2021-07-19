using System.ComponentModel.DataAnnotations;

using Svr.Core.Entities;
using Svr.Infrastructure.Identity;

namespace Svr.Web.Models.UsersViewModels
{
    public class ItemViewModel : ApplicationUser
    {
		/// <summary>
		/// Регион
		/// </summary>
		[Display(Name = "Регион")]
		public virtual string Region { get; set; }
		/// <summary>
		/// Район
		/// </summary>
		[Display(Name = "Район")]
		public virtual string District { get; set; }
		public string StatusMessage { get; set; }
    }
}
