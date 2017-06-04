#region Copyright Preamble

// 
//    Copyright @ 2017 NCode Group
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using Xunit;

namespace Unity.Enumerable.Tests
{
    public interface ITestComponent
    {
    }

    public class TestComponent0 : ITestComponent
    {
    }

    public class TestComponent1 : ITestComponent
    {
    }

    public class TestComponent2 : ITestComponent
    {
    }

    public class TestComponent3 : ITestComponent
    {
    }

    public class TestComponent4 : ITestComponent
    {
    }

    public class EnumerableTests
    {
        public static void RegisterEnumerable(IUnityContainer container, bool withDefault)
        {
            // ReSharper disable once ConvertToLocalFunction
            Func<LifetimeManager> transient = () => new TransientLifetimeManager();

            if (withDefault)
                container.RegisterType<ITestComponent, TestComponent0>(transient());

            container.RegisterType<ITestComponent, TestComponent1>("1", transient());
            container.RegisterType<ITestComponent, TestComponent2>("2", transient());
            container.RegisterType<ITestComponent, TestComponent3>("3", transient());
            container.RegisterType<ITestComponent, TestComponent4>("4", transient());
        }

        public static void AssertEnumerable(IUnityContainer container, bool withLazy, bool withDefault)
        {
            IList<ITestComponent> collection;
            if (withLazy)
            {
                var lazy = container.Resolve<Lazy<IEnumerable<ITestComponent>>>();
                Assert.False(lazy.IsValueCreated);

                collection = lazy.Value.ToList();
                Assert.True(lazy.IsValueCreated);
            }
            else
            {
                collection = container.Resolve<IEnumerable<ITestComponent>>().ToList();
            }

            var expectedCount = 4;
            var expectedAsserts = new List<Action<ITestComponent>>
            {
                item1 => Assert.IsType<TestComponent1>(item1),
                item2 => Assert.IsType<TestComponent2>(item2),
                item3 => Assert.IsType<TestComponent3>(item3),
                item4 => Assert.IsType<TestComponent4>(item4)
            };

            if (withDefault)
            {
                expectedCount += 1;
                expectedAsserts.Insert(0, item0 => Assert.IsType<TestComponent0>(item0));
            }

            Assert.Equal(expectedCount, collection.Count);
            Assert.Collection(collection, expectedAsserts.ToArray());
        }

        [Fact]
        public void ChildContainerEnumerable()
        {
            using (var root = new UnityContainer())
            {
                root.AddNewExtension<EnumerableExtension>();

                RegisterEnumerable(root, true);

                using (var child = root.CreateChildContainer())
                {
                    AssertEnumerable(child, false, true);
                }
            }
        }

        [Fact]
        public void ChildContainerLazyEnumerable()
        {
            using (var root = new UnityContainer())
            {
                root.AddNewExtension<EnumerableExtension>();

                RegisterEnumerable(root, true);

                using (var child = root.CreateChildContainer())
                {
                    AssertEnumerable(child, true, true);
                }
            }
        }

        [Fact]
        public void EnumerableWithExtension()
        {
            using (var container = new UnityContainer())
            {
                container.AddNewExtension<EnumerableExtension>();

                RegisterEnumerable(container, true);

                AssertEnumerable(container, false, true);
            }
        }

        [Fact]
        public void EnumerableWithoutExtension()
        {
            using (var container = new UnityContainer())
            {
                RegisterEnumerable(container, true);

                Assert.Throws<ResolutionFailedException>(() => AssertEnumerable(container, false, false));
            }
        }

        [Fact]
        public void LazyEnumerableWithExtension()
        {
            using (var container = new UnityContainer())
            {
                container.AddNewExtension<EnumerableExtension>();

                RegisterEnumerable(container, true);

                AssertEnumerable(container, true, true);
            }
        }

        [Fact]
        public void LazyEnumerableWithoutExtension()
        {
            using (var container = new UnityContainer())
            {
                RegisterEnumerable(container, true);

                AssertEnumerable(container, true, false);
            }
        }

        [Fact]
        public void LazyItem()
        {
            using (var container = new UnityContainer())
            {
                // ReSharper disable once ConvertToLocalFunction
                Func<LifetimeManager> transient = () => new TransientLifetimeManager();

                container.RegisterType<ITestComponent, TestComponent1>(transient());

                var lazy1 = container.Resolve<Lazy<ITestComponent>>();
                Assert.False(lazy1.IsValueCreated);

                var item = lazy1.Value;
                Assert.IsType<TestComponent1>(item);

                var lazy2 = container.Resolve<Lazy<ITestComponent>>();
                Assert.False(lazy2.IsValueCreated);
            }
        }
    }
}