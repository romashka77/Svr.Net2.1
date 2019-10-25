using Svr.Core.Entities;

namespace Svr.Core.Specifications
{
    public sealed class DirSpecification : BaseSpecification<Dir>
    {
        public DirSpecification(long? id) : base(i => (!id.HasValue || i.DirName.Id == id))
        {
            AddInclude(d => d.DirName);
        }
        public DirSpecification(string dirName) : base(i => (string.IsNullOrEmpty(dirName) || i.DirName.Name == dirName))
        {
            AddInclude(d => d.DirName);
        }
    }
}
