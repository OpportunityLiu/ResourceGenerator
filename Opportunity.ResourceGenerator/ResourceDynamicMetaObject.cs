using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace Opportunity.ResourceGenerator
{
    internal sealed class ResourceDynamicMetaObject : DynamicMetaObject
    {
        public ResourceDynamicMetaObject(Expression expression, IResourceProvider value)
            : base(expression, BindingRestrictions.Empty, value) { }

        private static readonly PropertyInfo ItemProperty = typeof(IResourceProvider).GetProperty("Item");
        private static readonly PropertyInfo ValueProperty = typeof(GeneratedResourceProvider).GetProperty("Value");
        private static readonly MethodInfo GetValueMethod = typeof(IResourceProvider).GetMethod("GetValue");

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            var parameters = new Expression[] { Expression.Constant(binder.Name) };
            var conv = Expression.Convert(this.Expression, typeof(IResourceProvider));
            var exp = Expression.Convert(Expression.Property(conv, ItemProperty, parameters), binder.ReturnType);
            return new DynamicMetaObject(exp, BindingRestrictions.GetTypeRestriction(Expression, this.LimitType));
        }

        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            if (args.Length != 0)
                return base.BindInvokeMember(binder, args);
            var conv = Expression.Convert(this.Expression, typeof(IResourceProvider));
            var parameters = new Expression[] { Expression.Constant(binder.Name) };
            var exp = Expression.Convert(Expression.Call(conv, GetValueMethod, parameters), binder.ReturnType);
            return new DynamicMetaObject(exp, BindingRestrictions.GetTypeRestriction(Expression, this.LimitType));
        }

        public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
        {
            if (args.Length != 0)
                base.BindInvoke(binder, args);
            var canConv = Expression.TypeIs(this.Expression, typeof(GeneratedResourceProvider));
            var conv = Expression.Convert(this.Expression, typeof(GeneratedResourceProvider));
            var cond = Expression.Condition(canConv, Expression.Property(conv, ValueProperty), Expression.Constant(string.Empty));
            var exp = Expression.Convert(cond, binder.ReturnType);
            return new DynamicMetaObject(exp, BindingRestrictions.GetTypeRestriction(Expression, this.LimitType));
        }
    }
}
