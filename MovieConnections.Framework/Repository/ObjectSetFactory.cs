using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using MovieConnections.Data.Models;
using MovieConnections.Framework.Utilities.Extensions;

namespace MovieConnections.Framework.Repository {
    public interface IObjectSetFactory {
        ObjectSet<T> CreateObjectSet<T>() where T : class, IEntity;
        void ChangeObjectState<T>(T entity, EntityState state) where T : class, IEntity;
        IEnumerable<TElement> ExecuteStoreQuery<TElement>(string functionName, object parameters = null);
        IEnumerable<TElement> ExecuteStoreQuery<TElement>(string functionName, params SqlParameter[] parameters);
    }

    public class ObjectSetFactory : IObjectSetFactory {
        private readonly ObjectContext _objectContext;
        private readonly Dictionary<string, object> _dictionary;

        public ObjectSetFactory(IObjectContextAdapter objectContext) {
            _objectContext = objectContext.ObjectContext;
        }

        public ObjectSet<T> CreateObjectSet<T>() where T : class, IEntity {
            return _objectContext.CreateObjectSet<T>();
        }

        public void ChangeObjectState<T>(T entity, EntityState state) where T : class, IEntity {
            _objectContext.ObjectStateManager.ChangeObjectState(entity, state);
        }

        public IEnumerable<TElement> ExecuteStoreQuery<TElement>(string functionName, object parameters = null)
        {
            if (parameters == null)
                return _objectContext.ExecuteStoreQuery<TElement>(functionName);

            var dictionary = parameters.ToDictionary();
            var keys = dictionary.Select(x => "@" + x.Key);
            var values = dictionary.Select(x => x.Value);

            var storedProcedure = string.Format("{0} {1}", functionName, string.Join(",", keys));
            return _objectContext.ExecuteStoreQuery<TElement>(storedProcedure, values);

        }

        public IEnumerable<TElement> ExecuteStoreQuery<TElement>(string functionName, params SqlParameter[] parameters)
        {
            var keys = parameters.Where(x => x != null).Select(x => "@" + x.ParameterName).ToArray();
            var format = string.Format("{0} {1}", functionName, string.Join(",", keys));

            return _objectContext.ExecuteStoreQuery<TElement>(format,
                parameters.Where(x => x != null).Cast<object>().ToArray()).ToList();
        }
    }
}