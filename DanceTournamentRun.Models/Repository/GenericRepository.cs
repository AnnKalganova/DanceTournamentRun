using DanceTournamentRun.Models.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DanceTournamentRun.Models.Repository
{
   
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// context of database
        /// </summary>
        private ApplicationDbContext context;

        /// <summary>
        /// allow access to context from inheritors
        /// </summary>
        protected ApplicationDbContext Context
        {
            get { return context; }
        }

        /// <summary>
        /// GenericRepository Constructor.
        /// </summary>
        /// <param name="context"></param>
        public GenericRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Returns all entities from database.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetAll(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = this.context.Set<TEntity>();
            // query.ToList();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            query = includeProperties.Split(
                new char[] { ',' },
                StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        /// <summary>
        /// Returns entity from database with Id===id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TEntity GetById(object id)
        {
            return this.context.Set<TEntity>().Find(id);
        }

        /// <summary>
        /// Check if there are any items in collection with next condition.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public bool Any(Expression<Func<TEntity, bool>> condition)
        {
            return this.context.Set<TEntity>().Any(condition);
        }

        /// <summary>
        /// Creates entity and add it to database.
        /// </summary>
        /// <param name="entity"></param>
        public void Create(TEntity entity)
        {
            this.context.Entry<TEntity>(entity).State = EntityState.Added;
            // this.context.Set<TEntity>().Add(entity);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Deletes entity from database.
        /// </summary>
        /// <param name="entityToDelete"></param>
        public void Delete(TEntity entityToDelete)
        {
            if (this.context.Entry(entityToDelete).State == EntityState.Detached)
            {
                this.context.Set<TEntity>().Attach(entityToDelete);
            }
            this.context.Set<TEntity>().Remove(entityToDelete);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Updates entity in database.
        /// </summary>
        /// <param name="entityToUpdate"></param>
        public void Update(TEntity entityToUpdate)
        {
            this.context.Set<TEntity>().Attach(entityToUpdate);
            this.context.Entry(entityToUpdate).State = EntityState.Modified;
            this.context.SaveChanges();
        }
    }
}
