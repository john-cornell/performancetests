using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ConsoleApplication1
{
    public delegate object ConstructorDelegate();

    public static class BetterActivator
    {
        public static ConstructorDelegate GetConstructor(string type)
        {
            Type t = Type.GetType(type);
            ConstructorInfo ctor = t.GetConstructor(new Type[0]);

            string methodName = t.Name + "Ctor";
            DynamicMethod dm = new DynamicMethod(methodName, t, new Type[0], typeof(Activator));
            ILGenerator lgen = dm.GetILGenerator();
            lgen.Emit(OpCodes.Newobj, ctor);
            lgen.Emit(OpCodes.Ret);

            ConstructorDelegate creator = (ConstructorDelegate)dm.CreateDelegate(typeof(ConstructorDelegate));

            return creator;
        }
    }
}
