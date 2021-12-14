using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ProjectCeleste.Misc.Utils.Extension
{
    public static class TypeExtensions
    {
        private static ConstructorInfo GetConstructor(this Type type,
            params Type[] argumentTypes)
        {
            type.ThrowIfNull(nameof(type));
            argumentTypes.ThrowIfNull(nameof(argumentTypes));

            var ci = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic, null,
                argumentTypes, null);
            if (ci != null)
                return ci;

            var sb = new StringBuilder();
            sb.Append(type.Name).Append(" has no ctor(");
            for (var i = 0; i < argumentTypes.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(',');
                }

                sb.Append(argumentTypes[i].Name);
            }

            sb.Append(')');
            throw new InvalidOperationException(sb.ToString());
        }

        public static Func<TResult> Ctor<TResult>(this Type type)
        {
            var ci = GetConstructor(type, Type.EmptyTypes);
            return Expression.Lambda<Func<TResult>>(
                Expression.New(ci)).Compile();
        }

        public static Func<TArg1, TResult>
            Ctor<TArg1, TResult>(this Type type)
        {
            var ci = GetConstructor(type, typeof(TArg1));
            var
                param1 = Expression.Parameter(typeof(TArg1), "arg1");

            return Expression.Lambda<Func<TArg1, TResult>>(
                Expression.New(ci, param1), param1).Compile();
        }

        public static Func<TArg1, TArg2, TResult>
            Ctor<TArg1, TArg2, TResult>(this Type type)
        {
            var ci = GetConstructor(type, typeof(TArg1), typeof(TArg2));
            ParameterExpression
                param1 = Expression.Parameter(typeof(TArg1), "arg1"),
                param2 = Expression.Parameter(typeof(TArg2), "arg2");

            return Expression.Lambda<Func<TArg1, TArg2, TResult>>(
                Expression.New(ci, param1, param2), param1, param2).Compile();
        }

        public static Func<TArg1, TArg2, TArg3, TResult>
            Ctor<TArg1, TArg2, TArg3, TResult>(this Type type)
        {
            var ci = GetConstructor(type, typeof(TArg1), typeof(TArg2), typeof(TArg3));
            ParameterExpression
                param1 = Expression.Parameter(typeof(TArg1), "arg1"),
                param2 = Expression.Parameter(typeof(TArg2), "arg2"),
                param3 = Expression.Parameter(typeof(TArg3), "arg3");

            return Expression.Lambda<Func<TArg1, TArg2, TArg3, TResult>>(
                Expression.New(ci, param1, param2, param3),
                param1, param2, param3).Compile();
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TResult>
            Ctor<TArg1, TArg2, TArg3, TArg4, TResult>(this Type type)
        {
            var ci = GetConstructor(type, typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4));
            ParameterExpression
                param1 = Expression.Parameter(typeof(TArg1), "arg1"),
                param2 = Expression.Parameter(typeof(TArg2), "arg2"),
                param3 = Expression.Parameter(typeof(TArg3), "arg3"),
                param4 = Expression.Parameter(typeof(TArg4), "arg4");

            return Expression.Lambda<Func<TArg1, TArg2, TArg3, TArg4, TResult>>(
                Expression.New(ci, param1, param2, param3, param4),
                param1, param2, param3, param4).Compile();
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>
            Ctor<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(this Type type)
        {
            var ci = GetConstructor(type, typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5));
            ParameterExpression
                param1 = Expression.Parameter(typeof(TArg1), "arg1"),
                param2 = Expression.Parameter(typeof(TArg2), "arg2"),
                param3 = Expression.Parameter(typeof(TArg3), "arg3"),
                param4 = Expression.Parameter(typeof(TArg4), "arg4"),
                param5 = Expression.Parameter(typeof(TArg5), "arg5");

            return Expression.Lambda<Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>>(
                Expression.New(ci, param1, param2, param3, param4, param5),
                param1, param2, param3, param4, param5).Compile();
        }
    }
}