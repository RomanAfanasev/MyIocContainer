using System;
using System.Collections.Generic;
using System.Text;

namespace MyIocContainer.Interfaces
{
    public interface IScope : IDisposable, IServiceProvider
    {
    }
}
