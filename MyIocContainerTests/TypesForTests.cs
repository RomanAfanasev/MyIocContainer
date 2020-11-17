using System;
using System.Collections.Generic;
using System.Text;

namespace MyIocContainerTests
{
    interface IFoo
    {
    }

    class Foo : IFoo
    {
    }

    interface IBar
    {
    }

    class Bar : IBar
    {
        public IFoo Foo { get; set; }

        public Bar(IFoo foo)
        {
            Foo = foo;
        }
    }

    interface IBaz
    {
    }

    class Baz : IBaz
    {
        public IFoo Foo { get; set; }
        public IBar Bar { get; set; }

        public Baz(IFoo foo, IBar bar)
        {  
            Foo = foo;
            Bar = bar;
        }
    }

    class SpyDisposable : IDisposable
    {
        public bool Disposed { get; private set; }

        public void Dispose() => Disposed = true;
    }
}
