using MyIocContainer.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyIocContainer
{
    public static class ContainerExtensions
    {
        public static IRegisteredType Register<T>(this Container container, Type type)
           => container.Register(typeof(T), type);

        public static IRegisteredType Register<TInterface, TImplementation>(this Container container)
           where TImplementation : TInterface
           => container.Register(typeof(TInterface), typeof(TImplementation));

        public static IRegisteredType Register<T>(this Container container, Func<T> factory)
            => container.Register(typeof(T), () => factory());

        public static IRegisteredType Register<T>(this Container container)
            => container.Register(typeof(T), typeof(T));

        public static T Resolve<T>(this IScope scope) => (T)scope.GetService(typeof(T));
    }
}
