using MyIocContainer.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MyIocContainer
{
    class ContainerLifetime : ObjectCache, ILifetime
    {        
        public Func<Type, Func<ILifetime, object>> GetFactory { get; private set; }

        public ContainerLifetime(Func<Type, Func<ILifetime, object>> getFactory) => GetFactory = getFactory;

        public object GetService(Type type) => GetFactory(type)(this);
                
        public object GetServiceAsSingleton(Type type, Func<ILifetime, object> factory)
            => GetCached(type, factory, this);
               
        public object GetServicePerScope(Type type, Func<ILifetime, object> factory)
            => GetServiceAsSingleton(type, factory);
    }
}
