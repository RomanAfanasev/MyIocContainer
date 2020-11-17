using MyIocContainer.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MyIocContainer
{
    public class Container : IScope
    {
        private readonly Dictionary<Type, Func<ILifetime, object>> _registeredTypes = new Dictionary<Type, Func<ILifetime, object>>();
        private readonly ContainerLifetime _lifetime;


        public Container() => _lifetime = new ContainerLifetime(t => _registeredTypes[t]);

        public IRegisteredType Register(Type @interface, Func<object> factory)
           => RegisterType(@interface, _ => factory());

        public IRegisteredType Register(Type @interface, Type implementation)
           => RegisterType(@interface, FactoryFromType(implementation));

        private IRegisteredType RegisterType(Type itemType, Func<ILifetime, object> factory)
            => new RegisteredType(itemType, f => _registeredTypes[itemType] = f, factory);

        public object GetService(Type type)
        {
            Func<ILifetime, object> registeredType;

            if (!_registeredTypes.TryGetValue(type, out registeredType))
            {
                return null;
            }

            return registeredType(_lifetime);
        }

        public IScope CreateScope() => new ScopeLifetime(_lifetime);

        public void Dispose() => _lifetime.Dispose();


        private static Func<ILifetime, object> FactoryFromType(Type itemType)
        {           
            var constructors = itemType.GetConstructors();
            if (constructors.Length == 0)
            {                
                constructors = itemType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            }
            var constructor = constructors.First();
                        
            var arg = Expression.Parameter(typeof(ILifetime));
            return (Func<ILifetime, object>)Expression.Lambda(
                Expression.New(constructor, constructor.GetParameters().Select(
                    param =>
                    {
                        var resolve = new Func<ILifetime, object>(
                            lifetime => lifetime.GetService(param.ParameterType));
                        return Expression.Convert(
                            Expression.Call(Expression.Constant(resolve.Target), resolve.Method, arg),
                            param.ParameterType);
                    })),
                arg).Compile();
        }
    }
}
