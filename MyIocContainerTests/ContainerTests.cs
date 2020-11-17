using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyIocContainer;
using System.Collections.Generic;

namespace MyIocContainerTests
{
    [TestClass]
    public class ContainerTests
    {
        private Container Container { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Container = new Container();
        }

        [TestMethod]
        public void SimpleReflectionConstruction()
        {
            Container.Register<IFoo>(typeof(Foo));

            object instance = Container.Resolve<IFoo>();
           
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void RecursiveReflectionConstruction()
        {
            Container.Register<IFoo>(typeof(Foo));
            Container.Register<IBar>(typeof(Bar));
            Container.Register<IBaz>(typeof(Baz));

            IBaz instance = Container.Resolve<IBaz>();
                      
            Assert.IsInstanceOfType(instance, typeof(Baz));

            var baz = instance as Baz;
            Assert.IsInstanceOfType(baz.Bar, typeof(Bar));
            Assert.IsInstanceOfType(baz.Foo, typeof(Foo));
        }

        [TestMethod]
        public void SimpleFactoryConstruction()
        {
            Container.Register<IFoo>(() => new Foo());

            object instance = Container.Resolve<IFoo>();
            
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void MixedConstruction()
        {
            Container.Register<IFoo>(() => new Foo());
            Container.Register<IBar>(typeof(Bar));
            Container.Register<IBaz>(typeof(Baz));

            IBaz instance = Container.Resolve<IBaz>();
                        
            Assert.IsInstanceOfType(instance, typeof(Baz));

            var baz = instance as Baz;
            Assert.IsInstanceOfType(baz.Bar, typeof(Bar));
            Assert.IsInstanceOfType(baz.Foo, typeof(Foo));
        }

        [TestMethod]
        public void InstanceResolution()
        {
            Container.Register<IFoo>(typeof(Foo));

            object instance1 = Container.Resolve<IFoo>();
            object instance2 = Container.Resolve<IFoo>();
                        
            Assert.AreNotEqual(instance1, instance2);
        }

        [TestMethod]
        public void SingletonResolution()
        {
            Container.Register<IFoo>(typeof(Foo)).AsSingleton();

            object instance1 = Container.Resolve<IFoo>();
            object instance2 = Container.Resolve<IFoo>();
                        
            Assert.AreEqual(instance1, instance2);
        }

        [TestMethod]
        public void PerScopeResolution()
        {
            Container.Register<IFoo>(typeof(Foo)).PerScope();

            object instance1 = Container.Resolve<IFoo>();
            object instance2 = Container.Resolve<IFoo>();
                        
            Assert.AreEqual(instance1, instance2);

            using (var scope = Container.CreateScope())
            {
                object instance3 = scope.Resolve<IFoo>();
                object instance4 = scope.Resolve<IFoo>();
                                
                Assert.AreEqual(instance3, instance4);
                                
                Assert.AreNotEqual(instance1, instance3);
            }
        }

        [TestMethod]
        public void MixedScopeResolution()
        {
            Container.Register<IFoo>(typeof(Foo)).PerScope();
            Container.Register<IBar>(typeof(Bar)).AsSingleton();
            Container.Register<IBaz>(typeof(Baz));

            using (var scope = Container.CreateScope())
            {
                Baz instance1 = scope.Resolve<IBaz>() as Baz;
                Baz instance2 = scope.Resolve<IBaz>() as Baz;
                               
                Assert.AreNotEqual(instance1, instance2);
                               
                Assert.AreEqual(instance1.Bar, instance2.Bar);
                Assert.AreEqual((instance1.Bar as Bar).Foo, (instance2.Bar as Bar).Foo);
                               
                Assert.AreEqual(instance1.Foo, instance2.Foo);
                                
                Assert.AreNotEqual(instance1.Foo, (instance1.Bar as Bar).Foo);
                Assert.AreNotEqual(instance2.Foo, (instance2.Bar as Bar).Foo);
            }
        }

        [TestMethod]
        public void SingletonScopedResolution()
        {
            Container.Register<IFoo>(typeof(Foo)).AsSingleton();
            Container.Register<IBar>(typeof(Bar)).PerScope();

            var instance1 = Container.Resolve<IBar>();

            using (var scope = Container.CreateScope())
            {
                var instance2 = Container.Resolve<IBar>();
                               
                Assert.AreEqual((instance1 as Bar).Foo, (instance2 as Bar).Foo);
            }
        }

        [TestMethod]
        public void MixedNoScopeResolution()
        {
            Container.Register<IFoo>(typeof(Foo)).PerScope();
            Container.Register<IBar>(typeof(Bar)).AsSingleton();
            Container.Register<IBaz>(typeof(Baz));

            Baz instance1 = Container.Resolve<IBaz>() as Baz;
            Baz instance2 = Container.Resolve<IBaz>() as Baz;
                       
            Assert.AreNotEqual(instance1, instance2);
                       
            Assert.AreEqual(instance1.Bar, instance2.Bar);
                       
            Assert.AreEqual(instance1.Foo, instance2.Foo);
            Assert.AreEqual(instance1.Foo, (instance1.Bar as Bar).Foo);
            Assert.AreEqual(instance2.Foo, (instance2.Bar as Bar).Foo);
        }

        [TestMethod]
        public void MixedReversedRegistration()
        {
            Container.Register<IBaz>(typeof(Baz));
            Container.Register<IBar>(typeof(Bar));
            Container.Register<IFoo>(() => new Foo());

            IBaz instance = Container.Resolve<IBaz>();
                       
            Assert.IsInstanceOfType(instance, typeof(Baz));

            var baz = instance as Baz;
            Assert.IsInstanceOfType(baz.Bar, typeof(Bar));
            Assert.IsInstanceOfType(baz.Foo, typeof(Foo));
        }

        [TestMethod]
        public void ScopeDisposesOfCachedInstances()
        {
            Container.Register<SpyDisposable>(typeof(SpyDisposable)).PerScope();
            SpyDisposable spy;

            using (var scope = Container.CreateScope())
            {
                spy = scope.Resolve<SpyDisposable>();
            }

            Assert.IsTrue(spy.Disposed);
        }

        [TestMethod]
        public void ContainerDisposesOfSingletons()
        {
            SpyDisposable spy;
            using (var container = new Container())
            {
                container.Register<SpyDisposable>().AsSingleton();
                spy = container.Resolve<SpyDisposable>();
            }

            Assert.IsTrue(spy.Disposed);
        }

        [TestMethod]
        public void SingletonsAreDifferentAcrossContainers()
        {
            var container1 = new Container();
            container1.Register<IFoo>(typeof(Foo)).AsSingleton();

            var container2 = new Container();
            container2.Register<IFoo>(typeof(Foo)).AsSingleton();

            Assert.AreNotEqual(container1.Resolve<IFoo>(), container2.Resolve<IFoo>());
        }

        [TestMethod]
        public void GetServiceUnregisteredTypeReturnsNull()
        {
            using (var container = new Container())
            {
                object value = container.GetService(typeof(Foo));

                Assert.IsNull(value);
            }
        }

        [TestMethod]
        public void GetServiceMissingDependencyThrows()
        {
            using (var container = new Container())
            {
                container.Register<Bar>();

                Assert.ThrowsException<KeyNotFoundException>(() => container.GetService(typeof(Bar)));
            }
        }
    }
}
