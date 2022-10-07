using System.Reflection;

namespace NetSecu_DemoReflection
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Test test = new Test(10);
            test.M1();
            Console.WriteLine();

            //Utilisation de la réflection
            Type testType = test.GetType(); //ou typeof(Test)

            //Récupère les membres accessibles (public et d'instance uniquement)
            IEnumerable<MemberInfo> members = testType.GetMembers();

            //Récupère les membres static accessibles (public)
            members = testType.GetMembers(BindingFlags.Public | BindingFlags.Static);

            //Récupère les membres accessibles (public static ou non)
            members = testType.GetMembers(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

            //Récupère les membres static accessibles y conpris ceux qui sont hérités (public)
            members = testType.GetMembers(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Static);

            //Récupère les membres accessibles y conpris ceux qui sont hérités (public ou non)
            members = testType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Instance);


            foreach (MemberInfo member in members.OrderBy(m => m.MemberType))
            {
                bool isStatic = false;
                switch(member.MemberType)
                {
                    case MemberTypes.Field:
                        isStatic = ((FieldInfo)member).IsStatic;
                        break;
                    case MemberTypes.Property:
                        isStatic = ((PropertyInfo)member).GetMethod?.IsStatic ?? false;
                        break;
                    case MemberTypes.Method:
                        isStatic = ((MethodInfo)member).IsStatic;
                        break;
                    case MemberTypes.Event:
                        isStatic = ((EventInfo)member).AddMethod?.IsStatic ?? false;
                        break;
                    case MemberTypes.Constructor:
                        isStatic = ((ConstructorInfo)member).IsStatic;
                        break;
                    case MemberTypes.NestedType:
                        Type memberType = (Type)member;
                        isStatic = memberType.IsAbstract && memberType.IsSealed;
                        break;
                }
                Console.WriteLine($"{member.Name} -> {member.MemberType} ({(isStatic ? "static" : "instance" )})");
            }

            //Attention Extremement dangereux puisqu'on by pass toutes les sécurités mise en place
            Console.WriteLine("Ce qui suit est extremement dangeureux");
            
            FieldInfo? fieldInfo = testType.GetField("_x", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if(fieldInfo is not null)
            {
                fieldInfo.SetValue(test, 5);
            }

            test.M1();
        }
    }

    class Test
    {
        public event Action<string>? Handler;

        private int _x;

        public int X
        {
            get
            {
                return _x;
            }

            private set
            {
                _x = value;
                Handler?.Invoke(nameof(X));
            }
        }

        public Test(int x)
        {
            X = x;
        }

        internal void M1()
        {
            Console.WriteLine($"M1 de Test (X : {X})");
        }

        struct Riri
        {

        }

        static class Toto
        {

        }
    }
}