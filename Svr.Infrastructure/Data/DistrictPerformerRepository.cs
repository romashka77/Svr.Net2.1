using Microsoft.EntityFrameworkCore;
using Svr.Core.Entities;
using Svr.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.Infrastructure.Data
{
    public class DistrictPerformerRepository : IDistrictPerformerRepository
    {
        private readonly DataContext dbContext;
        // ReSharper disable once InconsistentNaming
        private DbSet<DistrictPerformer> _entities;

        #region Ctor
        public DistrictPerformerRepository(DataContext dbContext)
        {
            this.dbContext = dbContext;
        }
        #endregion
        public virtual IEnumerable<DistrictPerformer> List(ISpecification<DistrictPerformer> spec)
        {
            // получение запроса, который включает в себя все выражения includes
            var queryableResultWithIncludes = spec.Includes.Aggregate(Entities.AsQueryable(), (current, include) => current.Include(include));

            // измените IQueryable, чтобы включить любые строковые операторы include
            var secondaryResult = spec.IncludeStrings.Aggregate(queryableResultWithIncludes, (current, include) => current.Include(include));

            // возвращает результат запроса с помощью выражения критериев спецификации
            return secondaryResult.Where(spec.Criteria).AsEnumerable();
        }
        public virtual async Task<List<DistrictPerformer>> ListAsync(ISpecification<DistrictPerformer> spec)
        {
            // получение запроса, который включает в себя все выражения includes
            var queryableResultWithIncludes = spec.Includes.Aggregate(Entities.AsQueryable(), (current, include) => current.Include(include));

            // измените IQueryable, чтобы включить любые строковые операторы include
            var secondaryResult = spec.IncludeStrings.Aggregate(queryableResultWithIncludes, (current, include) => current.Include(include));

            // возвращает результат запроса с помощью выражения критериев спецификации
            return await secondaryResult.Where(spec.Criteria).ToListAsync();
        }
        public virtual async Task<DistrictPerformer> AddAsync(DistrictPerformer entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            Entities.Add(entity);
            await dbContext.SaveChangesAsync();
            return entity;
        }
        public virtual async Task ClearAsync(ISpecification<DistrictPerformer> spec)
        {
            var list = List(spec);
            if (list != null)
            {
                foreach (var item in list)
                {
                    //await DeleteAsync(item);
                    Entities.Remove(item);
                }
                await dbContext.SaveChangesAsync();
            }
        }
        public virtual void Delete(DistrictPerformer entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            Entities.Remove(entity);
            //dbContext.Entry(entity).State = EntityState.Deleted;
            dbContext.SaveChanges();
        }
        public virtual async Task DeleteAsync(DistrictPerformer entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            //Entities.Remove(entity);
            dbContext.Entry(entity).State = EntityState.Deleted;
            await dbContext.SaveChangesAsync();
        }

        #region Properties
        /// <summary>
        /// Возвращает таблицу с включенной функцией "без отслеживания" (функция EF) используйте ее только при загрузке записей только для операций только для чтения
        /// </summary>
        public virtual IQueryable<DistrictPerformer> TableNoTracking { get { return Entities.AsNoTracking(); } }

        /// <summary>
        /// Entities
        /// </summary>
        protected virtual DbSet<DistrictPerformer> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = dbContext.Set<DistrictPerformer>();
                return _entities;
            }
        }
        #endregion

    }
}
