using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            int ITERATIONS = 10000;

            Console.WriteLine($"{ITERATIONS} iterations: ");

            Stopwatch watch = new Stopwatch();

            Dictionary<string, Action> actions = GetActions();

            foreach (KeyValuePair<string, Action> kvp in actions)
            {
                Console.WriteLine($"{kvp.Key}:");
                watch.Restart();

                for (int i = 0; i < ITERATIONS; i++)
                {
                    kvp.Value();
                }

                Console.WriteLine($"{watch.ElapsedMilliseconds} ms");
            }

            Console.ReadLine();
        }

        private static Dictionary<string, Action> GetActions()
        {
            Dictionary<string, Action> actions = new Dictionary<string, Action>();

            //actions["Six lines"] = StringNoInterpSixLines;
            //actions["One line"] = StringNoInterpOneLine;
            //actions["String Builder interp"] = StringBuilderInterp;

            //actions["String $ interp"] = StringDollarInterp;
            //actions["String format interp"] = StringFormatInterp;

            //actions["Get Generic Type"] = GetTypeByGeneric;            
            //actions["Get by name"] = GetTypeByName;

            actions["New static Constructor"] = CreateByStaticConstructor;
            actions["New local Constructor"] = CreateByLocalConstructor;
            actions["Create by type and cast"] = CreateByTypeAndCast;
            actions["Create by type and box"] = CreateByTypeAndBox;
            actions["Create by generic"] = CreateByGeneric;
            actions["Instantiate"] = Instatiate;

            return actions;
        }

        private static void StringNoInterpOneLine()
        {
            string first = "First String";
            string second = "Second String";
            string third = "Third String";

            string a = $"1st: " + first + "2nd: " + second + " 3rd: " + third;
        }

        private static void StringNoInterpSixLines()
        {
            string first = "First String";
            string second = "Second String";
            string third = "Third String";

            string a = $"1st: " + first + "2nd: " + second + " 3rd: " + third;

            a = a + first;
            a = a + "2nd: ";
            a = a + second;
            a = a + " 3rd: ";
            a = a + third;
        }

        private static void StringDollarInterp()
        {
            string first = "First String";
            string second = "Second String";
            string third = "Third String";

            string a = $"1st: {first} 2nd: {second} 3rd: {third}";
        }

        private static void StringFormatInterp()
        {
            string first = "First String";
            string second = "Second String";
            string third = "Third String";

            string a = string.Format("1st: {0} 2nd: {1} 3rd: {2}", first, second, third);
        }

        static StringBuilder builder = new StringBuilder();
        private static void StringBuilderInterp()
        {
            string first = "First String";
            string second = "Second String";
            string third = "Third String";

            builder.Clear();

            builder.Append("1st: ");
            builder.Append(first);

            builder.Append("2nd: ");
            builder.Append(second);

            builder.Append("3rd: ");
            builder.Append(third);


            string a = builder.ToString();
        }

        private static void GetTypeByGeneric()
        {
            Type t = typeof(StringBuilder);
        }

        private static void GetTypeByName()
        {
            Type t = Type.GetType("System.Text.StringBuilder");
        }

        private static void CreateByTypeAndCast()
        {
            StringBuilder builder = Activator.CreateInstance(typeof(StringBuilder)) as StringBuilder;
        }

        private static void CreateByStaticConstructor()
        {
            StringBuilder builder = BetterActivator.GetConstructor("System.Text.StringBuilder")() as StringBuilder;
        }

        private static void CreateByLocalConstructor()
        {
            ConstructorDelegate de = GetConstructor<StringBuilder>();
            StringBuilder builder = de() as StringBuilder;
        }

        private static void CreateByTypeAndBox()
        {
            StringBuilder builder = (StringBuilder)Activator.CreateInstance(typeof(StringBuilder));
        }

        private static void CreateByGeneric()
        {
            StringBuilder builder = Activator.CreateInstance<StringBuilder>();
        }

        private static void Instatiate()
        {
            StringBuilder builder = new StringBuilder();
        }

        static ConstructorDelegate GetConstructor<T>()
        {
            Type t = typeof(T);
            ConstructorInfo ctor = t.GetConstructor(new Type[0]);

            string methodName = t.Name + "Ctor";
            DynamicMethod dm = new DynamicMethod(methodName, t, new Type[0], typeof(Activator));
            ILGenerator lgen = dm.GetILGenerator();
            lgen.Emit(OpCodes.Newobj, ctor);
            lgen.Emit(OpCodes.Ret);

            ConstructorDelegate creator = (ConstructorDelegate)dm.CreateDelegate(typeof(ConstructorDelegate));

            return creator;
        }

        public delegate object ConstructorDelegate();
    }
}
