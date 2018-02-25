using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace Opportunity.ResourceGenerator
{
    internal sealed class ResourceDynamicMetaObject : DynamicMetaObject
    {
        public ResourceDynamicMetaObject(Expression expression, IResourceProvider value)
            : base(expression, BindingRestrictions.Empty, value)
        {
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            var parameters = new Expression[] { Expression.Constant(binder.Name) };

            return new DynamicMetaObject(Expression.Convert(Expression.Call(Expression.Convert(Expression, LimitType),
                typeof(IResourceProvider).GetProperty("Item").GetMethod,
                parameters), binder.ReturnType), BindingRestrictions.GetTypeRestriction(Expression, LimitType));
        }

        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            if (args.Length != 0)
                return base.BindInvokeMember(binder, args);
            var parameters = new Expression[] { Expression.Constant(binder.Name) };

            return new DynamicMetaObject(Expression.Convert(Expression.Call(Expression.Convert(Expression, LimitType),
                typeof(IResourceProvider).GetMethod("GetValue"),
                parameters), binder.ReturnType), BindingRestrictions.GetTypeRestriction(Expression, LimitType));
        }
    }
}
