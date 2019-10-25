using Svr.Core.Entities;

namespace Svr.Core.Specifications
{
    public sealed class FileEntitySpecification : BaseSpecification<FileEntity>
    {
        public FileEntitySpecification(long? id) : base(i => (!id.HasValue || i.Claim.Id == id))
        {
            AddInclude(d => d.Claim);
        }
    }
}
