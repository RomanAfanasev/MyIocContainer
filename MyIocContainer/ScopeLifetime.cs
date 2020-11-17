using MyIocContainer.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyIocContainer
{
    class ScopeLifetime : ObjectCache, ILifetime
    {        
        private readonly ContainerLifetime _parentLifetime;

        public ScopeLifetime(ContainerLifetime parentContainer) => _parentLifetime = parentContainer;

        public object GetService(Type type) => _parentLifetime.GetFactory(type)(this);
                
        public object GetServiceAsSingleton(Type type, Func<ILifetime, object> factory)
            => _parentLifetime.GetServiceAsSingleton(type, factory);
               
        public object GetServicePerScope(Type type, Func<ILifetime, object> factory)
            => GetCached(type, factory, this);
    }
}
