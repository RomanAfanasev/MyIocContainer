using System;
using System.Collections.Generic;
using System.Text;

namespace MyIocContainer.Interfaces
{
    interface ILifetime : IScope
    {
        object GetServiceAsSingleton(Type type, Func<ILifetime, object> factory);
        object GetServicePerScope(Type type, Func<ILifetime, object> factory);
    }
}
