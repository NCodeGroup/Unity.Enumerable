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

namespace Unity.Enumerable
{
    /// <summary>
    /// Provides an implementation of <see cref="IRegisteredNamesPolicy"/> that
    /// returns all the registrations for a given type with and without a name.
    /// The built-in implementation only returns the registrations that have names
    /// and doesn't return the default registration that doesn't have a name.
    /// </summary>
    public class RegisteredNamesPolicy : IRegisteredNamesPolicy
    {
        private readonly IUnityContainer _container;

        public RegisteredNamesPolicy(IUnityContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public IEnumerable<string> GetRegisteredNames(Type type)
        {
            return _container
                .Registrations
                .Where(_ => _.RegisteredType == type)
                .Select(_ => _.Name)
                .Distinct();
        }
    }
}