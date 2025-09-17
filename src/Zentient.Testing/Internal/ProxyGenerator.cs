using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Zentient.Testing.Internal
{
    internal static class ProxyGenerator
    {
        private static readonly AssemblyBuilder s_assemblyBuilder;
        private static readonly ModuleBuilder s_moduleBuilder;
        private static readonly ConcurrentDictionary<Type, Type> s_cache = new();

        static ProxyGenerator()
        {
            var name = new AssemblyName("Zentient.Testing.DynamicProxies");
            s_assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            s_moduleBuilder = s_assemblyBuilder.DefineDynamicModule(name.Name!);
        }

        public static T CreateProxyInstance<T>(MockEngine engine)
        {
            var iface = typeof(T);
            if (!iface.IsInterface) throw new ArgumentException("T must be an interface");

            var proxyType = s_cache.GetOrAdd(iface, CreateProxyType);
            return (T)Activator.CreateInstance(proxyType, engine)!;
        }

        private static Type CreateProxyType(Type iface)
        {
            string proxyName = $"Zentient_Testing_Proxy_{iface.Name}_{Guid.NewGuid():N}";
            var tb = s_moduleBuilder.DefineType(proxyName, TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class);
            tb.AddInterfaceImplementation(iface);

            // private readonly MockEngine _engine;
            var engineField = tb.DefineField("_engine", typeof(MockEngine), FieldAttributes.Private | FieldAttributes.InitOnly);

            // ctor(MockEngine engine) { base(); this._engine = engine; }
            var ctor = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(MockEngine) });
            var ilCtor = ctor.GetILGenerator();
            ilCtor.Emit(OpCodes.Ldarg_0);
            var objCtor = typeof(object).GetConstructor(Type.EmptyTypes)!;
            ilCtor.Emit(OpCodes.Call, objCtor);
            ilCtor.Emit(OpCodes.Ldarg_0);
            ilCtor.Emit(OpCodes.Ldarg_1);
            ilCtor.Emit(OpCodes.Stfld, engineField);
            ilCtor.Emit(OpCodes.Ret);

            // Implement methods
            foreach (var method in iface.GetMethods())
            {
                var paramInfos = method.GetParameters();
                var paramTypes = paramInfos.Select(p => p.ParameterType).ToArray();
                var mb = tb.DefineMethod(method.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot,
                    method.ReturnType,
                    paramTypes);

                var ilg = mb.GetILGenerator();

                // locals: object[] args, Type[] paramTypesArray, MethodInfo mi, object result
                var argsLocal = ilg.DeclareLocal(typeof(object[]));
                var paramTypesLocal = ilg.DeclareLocal(typeof(Type[]));
                var miLocal = ilg.DeclareLocal(typeof(MethodInfo));
                var resultLocal = ilg.DeclareLocal(typeof(object));

                // args = new object[paramCount]
                ilg.Emit(OpCodes.Ldc_I4, paramTypes.Length);
                ilg.Emit(OpCodes.Newarr, typeof(object));
                ilg.Emit(OpCodes.Stloc, argsLocal);

                // fill args
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    ilg.Emit(OpCodes.Ldloc, argsLocal);
                    ilg.Emit(OpCodes.Ldc_I4, i);
                    ilg.Emit(OpCodes.Ldarg, i + 1);
                    if (paramTypes[i].IsValueType)
                        ilg.Emit(OpCodes.Box, paramTypes[i]);
                    ilg.Emit(OpCodes.Stelem_Ref);
                }

                // load Type object for iface
                ilg.Emit(OpCodes.Ldtoken, iface);
                ilg.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static)!);

                // push method name
                ilg.Emit(OpCodes.Ldstr, method.Name);

                // build Type[] of parameter types
                ilg.Emit(OpCodes.Ldc_I4, paramTypes.Length);
                ilg.Emit(OpCodes.Newarr, typeof(Type));
                ilg.Emit(OpCodes.Stloc, paramTypesLocal);
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    ilg.Emit(OpCodes.Ldloc, paramTypesLocal);
                    ilg.Emit(OpCodes.Ldc_I4, i);
                    ilg.Emit(OpCodes.Ldtoken, paramTypes[i]);
                    ilg.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static)!);
                    ilg.Emit(OpCodes.Stelem_Ref);
                }

                // call GetMethod
                ilg.Emit(OpCodes.Ldloc, paramTypesLocal);
                ilg.Emit(OpCodes.Callvirt, typeof(Type).GetMethod("GetMethod", new Type[] { typeof(string), typeof(Type[]) })!);
                ilg.Emit(OpCodes.Stloc, miLocal);

                // load engine: this._engine
                ilg.Emit(OpCodes.Ldarg_0);
                ilg.Emit(OpCodes.Ldfld, engineField);

                // load methodinfo
                ilg.Emit(OpCodes.Ldloc, miLocal);

                // load args array
                ilg.Emit(OpCodes.Ldloc, argsLocal);

                // call engine.Invoke(methodInfo, args)
                ilg.Emit(OpCodes.Callvirt, typeof(MockEngine).GetMethod("Invoke")!);
                ilg.Emit(OpCodes.Stloc, resultLocal);

                // handle return
                if (method.ReturnType == typeof(void))
                {
                    // pop result and return
                    // result is in local, nothing to do
                }
                else
                {
                    // load resultLocal
                    ilg.Emit(OpCodes.Ldloc, resultLocal);
                    if (method.ReturnType.IsValueType)
                    {
                        ilg.Emit(OpCodes.Unbox_Any, method.ReturnType);
                    }
                    else
                    {
                        ilg.Emit(OpCodes.Castclass, method.ReturnType);
                    }
                    ilg.Emit(OpCodes.Ret);
                }

                // if void, just return
                if (method.ReturnType == typeof(void))
                {
                    ilg.Emit(OpCodes.Ret);
                }

                tb.DefineMethodOverride(mb, method);
            }

            return tb.CreateTypeInfo()!;
        }
    }
}
