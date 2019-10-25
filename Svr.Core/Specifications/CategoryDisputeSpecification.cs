using Svr.Core.Entities;
using System;
using System.Linq.Expressions;

namespace Svr.Core.Specifications
{
    public class CategoryDisputeSpecification : BaseSpecification<CategoryDispute>
    {
        public CategoryDisputeSpecification(Expression<Func<CategoryDispute, bool>> criteria) : base(criteria)
        {
        }
    }
}
