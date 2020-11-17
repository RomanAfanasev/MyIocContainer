using MyIocContainer.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MyIocContainer
{
    abstract class ObjectCache
    {       
        private readonly ConcurrentDictionary<Type, object> _instanceCache = new ConcurrentDictionary<Type, object>();
                
        protected object GetCached(Type type, Func<ILifetime, object> factory, ILifetime lifetime)
            => _instanceCache.GetOrAdd(type, _ => factory(lifetime));

        public void Dispose()
        {
            foreach (var obj in _instanceCache.Values)
                (obj as IDisposable)?.Dispose();
        }
    }
}
