using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DanceTournamentRun.Models.Repository.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Creates entity.
        /// </summary>
        /// <param name="entity"></param>
        void Create(TEntity entity);

        /// <summary>
        /// Returns all entities.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        IEnumerable<TEntity> GetAll(
            Expression<Func<TEntity,
            bool>> filter = null,
            Func<IQueryable<TEntity>,
            IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        /// <summary>
        /// Gets entity by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity GetById(object id);

        /// <summary>
        /// Updates entity.
        /// </summary>
        /// <param name="entityToUpdate"></param>
        void Update(TEntity entityToUpdate);

        /// <summary>
        /// Deletes entity.
        /// </summary>
        /// <param name="entityToDelete"></param>
        void Delete(TEntity entityToDelete);

        /// <summary>
        /// Check if there are any items in collection with next condition.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        bool Any(Expression<Func<TEntity, bool>> condition);
    }
}
