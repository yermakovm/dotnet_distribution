using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributionAPI.Model
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void Create(TEntity entity);
        void Delete(TEntity entity);
        void Delete(Guid id);
        void Delete();
        void Edit(TEntity entity);

        //read side (could be in separate Readonly Generic Repository)
        TEntity GetById(Guid id);
        IEnumerable<TEntity> Filter();
        IEnumerable<TEntity> Filter(Func<TEntity, bool> predicate);

        //separate method SaveChanges can be helpful when using this pattern with UnitOfWork
        Task SaveChanges();
    }
}
