using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QLESS.Core.Data.EntityFramework
{
    public class EntityFrameworkRepository : BaseRepository<IEntityFrameworkDbContext>, IRepository
    {
        // Constructors
        public EntityFrameworkRepository(IEntityFrameworkDbContext dbContext) : base(dbContext) { }

        // Methods
        public override TEntity Create<TEntity>(TEntity newEntity)
        {
            return DbContext
                .Set<TEntity>()
                .Add(newEntity)
                .Entity;
        }
        public override object Read<TKey>(Type type, TKey id)
        {
            var dbSet = DbContext
                .GetType()
                .GetMethods()
                .Single(m =>
                    m.Name == "Set" &&
                    m.GetGenericArguments().Length == 1 &&
                    m.GetParameters().Length == 0
                )
                .MakeGenericMethod(type)
                .Invoke(DbContext, null);

            return dbSet
                .GetType()
                .GetMethod("Find")
                .Invoke(dbSet, new object[] { new object[] { id } });
        }
        public override TEntity Read<TEntity, TKey>(TKey id)
        {
            return DbContext
                .Set<TEntity>()
                .Find(id);
        }
        public override IQueryable<TEntity> Read<TEntity>()
        {
            return DbContext.Set<TEntity>();
        }
        public override IQueryable<TEntity> Read<TEntity>(Expression<Func<TEntity, bool>> predicate)
        {
            return DbContext
                .Set<TEntity>()
                .Where(predicate);
        }
        public override IQueryable<TEntity> Read<TEntity>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includedProperties)
        {
            return SetIncludedProperties(DbContext.Set<TEntity>(), includedProperties).Where(predicate);
        }
        public override IQueryable<TEntity> Read<TEntity>(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize)
        {
            return DbContext
                .Set<TEntity>()
                .Where(predicate)
                .OrderBy(e => e)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }
        public override IQueryable<TEntity> Read<TEntity>(Expression<Func<TEntity, bool>> predicate, int pageNumber, int pageSize, params Expression<Func<TEntity, object>>[] includedProperties)
        {
            return SetIncludedProperties(DbContext.Set<TEntity>().AsQueryable(), includedProperties)
                .Where(predicate)
                .OrderBy(e => e)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }
        public override TEntity Update<TEntity>(TEntity entity)
        {
            if (entity != null)
            {
                if (DbContext.Entry(entity).State == EntityState.Detached)
                {
                    DbContext.Set<TEntity>().Attach(entity);
                }
                DbContext.Entry(entity).State = EntityState.Modified;
            }

            return entity;
        }
        public override void Delete<TEntity>(TEntity entity)
        {
            if (DbContext.Entry(entity).State == EntityState.Detached)
            {
                DbContext.Set<TEntity>().Attach(entity);
            }
            DbContext.Set<TEntity>().Remove(entity);
        }
        public override void Delete<TEntity, TKey>(TKey id)
        {
            TEntity entityToDelete = DbContext
                .Set<TEntity>()
                .Find(id);
            Delete(entityToDelete);
        }
        public override TEntity Attach<TEntity>(TEntity entity)
        {
            return DbContext.Entry(entity).State == EntityState.Detached
                ? DbContext.Set<TEntity>().Attach(entity).Entity
                : entity;
        }
        public override int SaveChanges()
        {
            return DbContext.SaveChanges();
        }
        public override Type GetObjectType(object entity)
        {
            return DbContext.Model.FindRuntimeEntityType(entity.GetType()).ClrType;
        }

        // Private Methods
        private IQueryable<TEntity> SetIncludedProperties<TEntity>(IQueryable<TEntity> entities, params Expression<Func<TEntity, object>>[] includedProperties)
            where TEntity : class
        {
            foreach (var i in includedProperties)
            {
                entities = entities.Include(i);
            }
            return entities;
        }
    }
}
