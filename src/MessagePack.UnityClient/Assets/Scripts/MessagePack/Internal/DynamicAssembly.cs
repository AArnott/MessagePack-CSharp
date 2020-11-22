// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if !(UNITY_2018_3_OR_NEWER && NET_STANDARD_2_0)

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace MessagePack.Internal
{
    internal class DynamicAssembly
    {
        private readonly AssemblyBuilder assemblyBuilder;
        private readonly ModuleBuilder moduleBuilder;

        // don't expose ModuleBuilder
        //// public ModuleBuilder ModuleBuilder { get { return moduleBuilder; } }

        public DynamicAssembly(string moduleName)
        {
            AssemblyBuilderAccess builderAccess = AssemblyBuilderAccess.Run;
            this.assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(moduleName), builderAccess);
            this.moduleBuilder = this.assemblyBuilder.DefineDynamicModule(moduleName + ".dll");
        }

        internal Assembly Assembly => this.assemblyBuilder;

        /* requires lock on mono environment. see: https://github.com/neuecc/MessagePack-CSharp/issues/161 */

        public TypeBuilder DefineType(string name, TypeAttributes attr) => this.moduleBuilder.DefineType(name, attr);

        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent) => this.moduleBuilder.DefineType(name, attr, parent);

        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, Type[] interfaces) => this.moduleBuilder.DefineType(name, attr, parent, interfaces);
    }
}

#endif
