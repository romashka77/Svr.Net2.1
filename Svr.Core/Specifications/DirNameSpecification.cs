using Svr.Core.Entities;
using System;
using System.Linq.Expressions;

namespace Svr.Core.Specifications
{
    public class DirNameSpecification : BaseSpecification<DirName>
    {
        public DirNameSpecification(Expression<Func<DirName, bool>> criteria) : base(criteria)
        {
        }
    }
}
