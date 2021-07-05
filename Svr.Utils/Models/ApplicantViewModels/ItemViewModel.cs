using Svr.Core.Entities;

namespace Svr.Utils.Models.ApplicantViewModels
{
    public class ItemViewModel : Applicant
    {
        public bool IsMan { get; set; }
        public string StatusMessage { get; set; }
    }
}
