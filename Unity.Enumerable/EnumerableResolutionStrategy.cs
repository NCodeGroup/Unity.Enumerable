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
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace Unity.Enumerable
{
    public class EnumerableResolutionStrategy : BuilderStrategy
    {
        private static readonly MethodInfo GenericResolveEnumerableMethod =
            typeof(EnumerableResolutionStrategy).GetMethod(
                nameof(ResolveEnumerable),
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public override void PreBuildUp(IBuilderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!IsResolvingEnumerable(context.BuildKey.Type)) return;

            var typeToBuild = context.BuildKey.Type;
            var itemType = typeToBuild.GetGenericArguments()[0];
            var contractType = IsResolvingLazy(itemType) ? itemType.GetGenericArguments()[0] : itemType;
            var resolverMethod = GenericResolveEnumerableMethod.MakeGenericMethod(contractType, itemType);
            var resolver = (Func<IBuilderContext, object>)Delegate.CreateDelegate(typeof(Func<IBuilderContext, object>), resolverMethod);

            context.Existing = resolver(context);
            context.BuildComplete = true;
        }

        private static bool IsResolvingEnumerable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        private static bool IsResolvingLazy(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Lazy<>);
        }

        private static IEnumerable<TItem> ResolveEnumerable<TContract, TItem>(IBuilderContext context)
        {
            var type = typeof(TContract);
            var container = context.NewBuildUp<IUnityContainer>();
            var registeredNamesPolicy = context.Policies.Get<IRegisteredNamesPolicy>(null);
            var names = registeredNamesPolicy.GetRegisteredNames(type);

            if (type.IsGenericType)
                names = registeredNamesPolicy.GetRegisteredNames(type.GetGenericTypeDefinition());

            return names
                .Select(name => container.Resolve<TItem>(name))
                .ToList()
                .AsReadOnly();
        }
    }
}