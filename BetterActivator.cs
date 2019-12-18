using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ConsoleApplication1
{
    public delegate object ConstructorDelegate();

    public static class BetterActivator
    {
        static Dictionary<Type, ConstructorDelegate> _constructorCache = new Dictionary<Type, ConstructorDelegate>();

        public static ConstructorDelegate GetConstructor(string type)
        {
            Type t = Type.GetType(type);
            if (_constructorCache.ContainsKey(t)) return _constructorCache[t];

            ConstructorInfo ctor = t.GetConstructor(new Type[0]);

            string methodName = t.Name + "Ctor";
            DynamicMethod dm = new DynamicMethod(methodName, t, new Type[0], typeof(Activator));
            ILGenerator lgen = dm.GetILGenerator();
            lgen.Emit(OpCodes.Newobj, ctor);
            lgen.Emit(OpCodes.Ret);

            ConstructorDelegate creator = (ConstructorDelegate)dm.CreateDelegate(typeof(ConstructorDelegate));

            _constructorCache[t] = creator;

            return creator;
        }
    }
}
