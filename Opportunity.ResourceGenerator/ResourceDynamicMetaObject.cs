using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Opportunity.ResourceGenerator
{
    internal sealed class ResourceDynamicMetaObject : DynamicMetaObject
    {
        public ResourceDynamicMetaObject(Expression expression, IResourceProvider value)
            : base(expression, BindingRestrictions.GetTypeRestriction(expression, value.GetType()), value)
        {
            foreach (var item in LimitType.GetInterfaces())
            {
                if (LimitType.GetTypeInfo().Module != item.GetTypeInfo().Module)
                    continue;
                var methods = item.GetMethods();
                this.Interfaces[item] = methods;
            }
        }

        private static readonly PropertyInfo ItemProperty = typeof(IResourceProvider).GetProperty("Item");
        private static readonly PropertyInfo ValueProperty = typeof(GeneratedResourceProvider).GetProperty("Value");
        private static readonly MethodInfo GetValueMethod = typeof(IResourceProvider).GetMethod("GetValue");
        private readonly Dictionary<Type, MethodInfo[]> Interfaces = new Dictionary<Type, MethodInfo[]>();

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            var parameters = new Expression[] { Expression.Constant(binder.Name) };
            var conv = Expression.Convert(this.Expression, typeof(IResourceProvider));
            var exp = Expression.Convert(Expression.Property(conv, ItemProperty, parameters), binder.ReturnType);
            return new DynamicMetaObject(exp, Restrictions);
        }

        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            if (args.Length != 0)
            {
                var intf = default(Type);
                var method = default(MethodInfo);
                var param = default(ParameterInfo[]);
                var ignoreCase = binder.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                foreach (var item in this.Interfaces)
                {
                    foreach (var m in item.Value)
                    {
                        if (string.Equals(m.Name, binder.Name, ignoreCase))
                        {
                            var p = m.GetParameters();
                            var meet = false;
                            if (p.Length == args.Length)
                            {
                                meet = true;
                                for (var i = 0; i < p.Length; i++)
                                {
                                    if (!p[i].ParameterType.IsAssignableFrom(args[i].LimitType))
                                    {
                                        meet = false;
                                        break;
                                    }
                                }
                            }
                            if (meet)
                            {
                                intf = item.Key;
                                method = m;
                                param = p;
                                break;

                            }
                        }
                    }
                    if (method != null)
                        break;
                }
                if (method == null)
                    return base.BindInvokeMember(binder, args);
                var convert = Expression.Convert(this.Expression, intf);
                var paramExp = new Expression[param.Length];
                var restrictions = Restrictions;
                for (var i = 0; i < paramExp.Length; i++)
                {
                    paramExp[i] = Expression.Convert(args[i].Expression, param[i].ParameterType);
                    if (args[i].RuntimeType != null)
                        restrictions = restrictions.Merge(BindingRestrictions.GetTypeRestriction(args[i].Expression, args[i].LimitType));
                }
                var expM = Expression.Convert(Expression.Call(convert, method, paramExp), binder.ReturnType);
                return new DynamicMetaObject(expM, restrictions);
            }
            var conv = Expression.Convert(this.Expression, typeof(IResourceProvider));
            var parameters = new Expression[] { Expression.Constant(binder.Name) };
            var exp = Expression.Convert(Expression.Call(conv, GetValueMethod, parameters), binder.ReturnType);
            return new DynamicMetaObject(exp, Restrictions);
        }

        public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
        {
            if (args.Length != 0)
                base.BindInvoke(binder, args);
            var canConv = Expression.TypeIs(this.Expression, typeof(GeneratedResourceProvider));
            var conv = Expression.Convert(this.Expression, typeof(GeneratedResourceProvider));
            var cond = Expression.Condition(canConv, Expression.Property(conv, ValueProperty), Expression.Constant(string.Empty));
            var exp = Expression.Convert(cond, binder.ReturnType);
            return new DynamicMetaObject(exp, Restrictions);
        }
    }
}
