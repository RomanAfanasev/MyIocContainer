using MyIocContainer.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyIocContainer
{
    class RegisteredType : IRegisteredType
    {
        private readonly Type _itemType;
        private readonly Action<Func<ILifetime, object>> _registerFactory;
        private readonly Func<ILifetime, object> _factory;

        public RegisteredType(Type itemType, Action<Func<ILifetime, object>> registerFactory, Func<ILifetime, object> factory)
        {
            _itemType = itemType;
            _registerFactory = registerFactory;
            _factory = factory;

            registerFactory(_factory);
        }

        public void AsSingleton()
            => _registerFactory(lifetime => lifetime.GetServiceAsSingleton(_itemType, _factory));

        public void PerScope()
            => _registerFactory(lifetime => lifetime.GetServicePerScope(_itemType, _factory));
    }
}
