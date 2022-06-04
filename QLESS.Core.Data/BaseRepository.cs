using System;
using System.Linq;
using System.Linq.Expressions;

namespace QLESS.Core.Data
{
    public abstract class BaseRepository<TContext> : IRepository
        where TContext : IDbContext
    {
        // Properties
        protected TContext DbContext { get; }

        // Constructor
        protected BaseRepository(TContext dbContext)
        {
            DbContext = dbContext;
        }

        // Methods
        public abstract TEntity Create<TEntity>(TEntity newEntity) where TEntity : class;
        public abstract object Read<TKey>(Type type, TKey id) where TKey : struct;
        public abstract TEntity Read<TEntity, TKey>(TKey id) where TEntity : class where TKey : struct;
        public abstract IQueryable<TEntity> Read<TEntity>() where TEntity : class;
        public abstract IQueryable<TEntity> Read<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        public abstract IQueryable<TEntity> Read<TEntity>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includedProperties) where TEntity : class;
        public abstract IQueryable<TEntity> Read<TEntity>(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize) where TEntity : class;
        public abstract IQueryable<TEntity> Read<TEntity>(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize, params Expression<Func<TEntity, object>>[] includedProperties) where TEntity : class;
        public abstract TEntity Update<TEntity>(TEntity entity) where TEntity : class;
        public abstract void Delete<TEntity>(TEntity entity) where TEntity : class;
        public abstract void Delete<TEntity, TKey>(TKey id) where TEntity : class where TKey : struct;
        public abstract TEntity Attach<TEntity>(TEntity entity) where TEntity : class;
        public abstract int SaveChanges();
        public abstract Type GetObjectType(object entity);
    }
}
