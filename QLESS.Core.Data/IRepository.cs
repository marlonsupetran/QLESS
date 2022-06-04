using System;
using System.Linq;
using System.Linq.Expressions;

namespace QLESS.Core.Data
{
    public interface IRepository
    {
        TEntity Create<TEntity>(TEntity newEntity) where TEntity : class;
        object Read<TKey>(Type type, TKey id) where TKey : struct;
        TEntity Read<TEntity, TKey>(TKey id) where TEntity : class where TKey : struct;
        IQueryable<TEntity> Read<TEntity>() where TEntity : class;
        IQueryable<TEntity> Read<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        IQueryable<TEntity> Read<TEntity>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includedProperties) where TEntity : class;
        IQueryable<TEntity> Read<TEntity>(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize) where TEntity : class;
        IQueryable<TEntity> Read<TEntity>(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize, params Expression<Func<TEntity, object>>[] includedProperties) where TEntity : class;
        TEntity Update<TEntity>(TEntity entity) where TEntity : class;
        void Delete<TEntity>(TEntity entity) where TEntity : class;
        void Delete<TEntity, TKey>(TKey id) where TEntity : class where TKey : struct;
        TEntity Attach<TEntity>(TEntity entity) where TEntity : class;
        int SaveChanges();
        Type GetObjectType(object entity);
    }
}
