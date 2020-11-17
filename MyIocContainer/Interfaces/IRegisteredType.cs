using System;
using System.Collections.Generic;
using System.Text;

namespace MyIocContainer.Interfaces
{
    public interface IRegisteredType
    {
        void AsSingleton();
        void PerScope();
    }
}
