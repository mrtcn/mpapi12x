using System;
using System.Data.Entity;
using System.Threading.Tasks;
using MovieConnections.Data.Models;

namespace MovieConnections.Framework.Repository {
    public interface IUnitOfWork : IDisposable {
        void CommitChanges();
        Task CommitChangesAsync();
        DbSet<T> CreateObjectSet<T>() where T : class, IEntity;
        void ChangeObjectState<T>(T entity, EntityState state) where T : class, IEntity;
    }

    public class UnitOfWork : IUnitOfWork {
        private readonly ApplicationDbContext _objectContext;

        public UnitOfWork(ApplicationDbContext objectContext) {
            _objectContext = objectContext;
        }

        public DbSet<T> CreateObjectSet<T>() where T : class, IEntity
        {
            return _objectContext.Set<T>();
        }

        public void ChangeObjectState<T>(T entity, EntityState state) where T : class, IEntity
        {
            _objectContext.Entry(entity).State = state;
        }

        public void CommitChanges() {
            _objectContext.SaveChanges();
        }

        public async Task CommitChangesAsync() {
            await _objectContext.SaveChangesAsync();
        }
        private bool Disposed { get; set; }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!Disposed) {
                if (disposing)
                    _objectContext?.Dispose();
            }
            Disposed = true;
        }
    }
}