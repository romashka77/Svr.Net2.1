using Svr.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Svr.Core.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        T GetById(long? id);
        T GetSingleBySpec(ISpecification<T> spec);
        IQueryable<T> ListAll();
        IQueryable<T> List(ISpecification<T> spec);
        T Add(T entity);
        int Add(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(T entity);
        bool EntityExists(long id);
        IQueryable<T> Table();
        IQueryable<T> TableNoTracking();
    }
}
