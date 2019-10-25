using Svr.Core.Entities;

namespace Svr.Web.Models.ApplicantViewModels
{
    public class ItemViewModel : Applicant
    {
        public bool IsMan { get; set; }
        public string StatusMessage { get; set; }
    }
}
