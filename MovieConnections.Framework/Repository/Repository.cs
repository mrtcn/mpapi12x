using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using MovieConnections.Data.Models;

namespace MovieConnections.Framework.Repository {
    public interface IRepository<TEntity> where TEntity : class, IEntity {
        TEntity Get(int id);
        IQueryable<TEntity> Table();
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate = null);
        TEntity Create(TEntity entity);
        TEntity Update(TEntity entity);
        void Delete(int id);
        void SaveChanges();
    }

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity{
        private readonly DbSet<TEntity> _objectSet;
        private readonly IUnitOfWork _unitOfWork;

        public Repository(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
            _objectSet = unitOfWork.CreateObjectSet<TEntity>();
        }

        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null)
                return Table().FirstOrDefault();
            return Table().FirstOrDefault(predicate);
        }

        public virtual IQueryable<TEntity> Table() {
            return _objectSet;
        }

        public virtual TEntity Get(int id) {
            return FirstOrDefault(x => x.Id == id);
        }

        public virtual TEntity Create(TEntity entity) {
            _objectSet.Add(entity);
            return entity;
        }

        public virtual TEntity Update(TEntity entity) {            
            _objectSet.Attach(entity);
            _unitOfWork.ChangeObjectState(entity, EntityState.Modified);

            return entity;
        }

        public virtual void Delete(int id) {
            var entity = FirstOrDefault(x => x.Id == id);
            _objectSet.Remove(entity);
            _unitOfWork.ChangeObjectState(entity, EntityState.Deleted);

        }

        public void SaveChanges() {
            _unitOfWork.CommitChanges();
        }

    }
}