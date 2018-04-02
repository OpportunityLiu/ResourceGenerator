using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Dynamic;
using System.Linq.Expressions;
using Microsoft.CSharp.RuntimeBinder;

namespace Opportunity.ResourceGenerator
{
    /// <summary>
    /// Analyze interpolation string.
    /// </summary>
    [DebuggerDisplay(@"${DebugDisplay}")]
    public sealed class FormattableResourceString
    {
        private class DynamicCaller
        {
            private readonly CallSite<Func<CallSite, object, string, object>> indexCache;
            private readonly Dictionary<string, CallSite<Func<CallSite, object, object>>> memberCache
                = new Dictionary<string, CallSite<Func<CallSite, object, object>>>();
            private readonly Type type;

            public DynamicCaller(Type type)
            {
                this.type = type;
                this.indexCache = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, type, new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null) }));
            }

            public object Get(object param, string name)
            {
                try
                {
                    return this.indexCache.Target(this.indexCache, param, name);
                }
                catch { }
                if (this.memberCache.TryGetValue(name, out var callSite) && callSite != null)
                {
                    try
                    {
                        return callSite.Target(callSite, param);
                    }
                    catch { }
                }
                try
                {
                    callSite = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, name, this.type, new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
                    var r = callSite.Target(callSite, param);
                    this.memberCache[name] = callSite;
                    return r;
                }
                catch { }
                return null;
            }
        }

        private static readonly Dictionary<Type, DynamicCaller> callers = new Dictionary<Type, DynamicCaller>();

        private static string analyze(string format, IList<string> arguments)
        {
            var sb = new StringBuilder(format.Length);
            var inBrance = false;
            var currentArg = new StringBuilder(10);
            for (var i = 0; i < format.Length; i++)
            {
                switch (format[i])
                {
                case '{':
                    if (i + 1 < format.Length && format[i + 1] == '{')
                    {
                        if (inBrance)
                            currentArg.Append("{{");
                        else
                            sb.Append("{{");
                        i++;
                    }
                    else if (!inBrance)
                    {
                        inBrance = true;
                        sb.Append('{');
                    }
                    else
                    {
                        throw new ArgumentException($"Another '{{' expected at postion {i}.", nameof(format));
                    }
                    break;
                case '}':
                    if (i + 1 < format.Length && format[i + 1] == '}')
                    {
                        if (inBrance)
                            currentArg.Append("}}");
                        else
                            sb.Append("}}");
                        i++;
                    }
                    else if (inBrance)
                    {
                        inBrance = false;
                        var arg = currentArg.ToString();
                        currentArg.Clear();
                        var argsp = arg.IndexOfAny(new char[] { ':', ',' });
                        var argFormat = "";
                        if (argsp > 0)
                        {
                            argFormat = arg.Substring(argsp);
                            arg = arg.Substring(0, argsp);
                        }
                        arg = arg.Trim();
                        var argIndex = arguments.IndexOf(arg);
                        if (argIndex < 0)
                        {
                            argIndex = arguments.Count;
                            arguments.Add(arg);
                        }
                        sb.Append(argIndex).Append(argFormat)
                            .Append('}');
                    }
                    else
                    {
                        throw new ArgumentException($"Another '}}' expected at postion {i}.", nameof(format));
                    }
                    break;
                default:
                    if (inBrance)
                        currentArg.Append(format[i]);
                    else
                        sb.Append(format[i]);
                    break;
                }
            }
            if (inBrance)
                throw new ArgumentException($"Another '}}' expected at postion {format.Length}.", nameof(format));
            return sb.ToString();
        }

        /// <summary>
        /// Argument in <see cref="FormattableResourceString"/>
        /// </summary>
        [DebuggerDisplay(@"\{{Index}\} = {Name}")]
        public readonly struct Argument
        {
            internal Argument(string name, int index)
            {
                this.Name = name;
                this.Index = index;
            }

            /// <summary>
            /// Name of argument.
            /// </summary>
            public string Name { get; }
            /// <summary>
            /// Index used in <see cref="FormatString"/>.
            /// </summary>
            public int Index { get; }
        }

        /// <summary>
        /// Create new instance of <see cref="FormattableResourceString"/>.
        /// </summary>
        /// <param name="format">Interpolation string.</param>
        public FormattableResourceString(string format)
        {
            if (string.IsNullOrEmpty(format))
            {
                this.FormatString = "";
                return;
            }
            var args = new List<string>();
            FormatString = analyze(format, args);
            if (args.Count == 0)
            {
                this.arguments = Array.Empty<Argument>();
            }
            else if (args.Count == 1)
            {
                this.arguments = new[] { new Argument(args[0], 0) };
            }
            else
            {
                var oargs = args.ToArray();
                Array.Sort(oargs);
                FormatString = analyze(format, oargs);
                var argData = new Argument[args.Count];
                for (var i = 0; i < argData.Length; i++)
                {
                    var arg = args[i];
                    var index = Array.IndexOf(oargs, arg);
                    argData[i] = new Argument(arg, index);
                }
                this.arguments = argData;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebugDisplay
        {
            get
            {
                var sb = new StringBuilder(FormatString.Length);
                var inBrance = false;
                var currentArg = new StringBuilder(10);
                for (var i = 0; i < FormatString.Length; i++)
                {
                    switch (FormatString[i])
                    {
                    case '{':
                        if (i + 1 < FormatString.Length && FormatString[i + 1] == '{')
                        {
                            if (inBrance)
                                currentArg.Append("{{");
                            else
                                sb.Append("{{");
                            i++;
                        }
                        else if (!inBrance)
                        {
                            inBrance = true;
                            sb.Append('{');
                        }
                        else
                        {
                            return FormatString;
                        }
                        break;
                    case '}':
                        if (i + 1 < FormatString.Length && FormatString[i + 1] == '}')
                        {
                            if (inBrance)
                                currentArg.Append("}}");
                            else
                                sb.Append("}}");
                            i++;
                        }
                        else if (inBrance)
                        {
                            inBrance = false;
                            var arg = currentArg.ToString();
                            currentArg.Clear();
                            var argsp = arg.IndexOfAny(new char[] { ':', ',' });
                            var argFormat = "";
                            if (argsp > 0)
                            {
                                argFormat = arg.Substring(argsp);
                                arg = arg.Substring(0, argsp);
                            }
                            var argIndex = int.Parse(arg);
                            sb.Append(Arguments.First(a => a.Index == argIndex).Name).Append(argFormat)
                                .Append('}');
                        }
                        else
                        {
                            return FormatString;
                        }
                        break;
                    default:
                        if (inBrance)
                            currentArg.Append(FormatString[i]);
                        else
                            sb.Append(FormatString[i]);
                        break;
                    }
                }
                if (inBrance)
                    return FormatString;
                return sb.ToString();
            }
        }

        /// <summary>
        /// Format string used for <see cref="string.Format(string, object[])"/>.
        /// </summary>
        public string FormatString { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Argument[] arguments;
        /// <summary>
        /// Arguments of the <see cref="FormattableResourceString"/>, ordered by its appearance.
        /// </summary>
        public IReadOnlyList<Argument> Arguments => new ReadOnlyCollection<Argument>(this.arguments);

        /// <summary>
        /// Returns <see cref="FormatString"/>.
        /// </summary>
        /// <returns>Value of <see cref="FormatString"/>.</returns>
        public override string ToString() => FormatString;

        private object[] createArguments(object parameters)
        {
            if (this.arguments.Length == 0)
                return Array.Empty<object>();
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var type = parameters.GetType();
            var args = new object[this.arguments.Length];
            if (!callers.TryGetValue(type, out var caller))
            {
                caller = new DynamicCaller(type);
                callers[type] = caller;
            }
            foreach (var item in this.arguments)
            {
                args[item.Index] = caller.Get(parameters, item.Name);
            }
            return args;
        }

        /// <summary>
        /// Format string with given parameters.
        /// </summary>
        /// <param name="parameters">An object contains parameters.</param>
        /// <param name="provider">A format provider for formatting.</param>
        /// <returns>A formatted string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
        public string Format(IFormatProvider provider, object parameters)
            => string.Format(provider, FormatString, createArguments(parameters));

        /// <summary>
        /// Format string with given parameters.
        /// </summary>
        /// <param name="parameters">An object contains parameters.</param>
        /// <returns>A formatted string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
        public string Format(object parameters) => Format(null, parameters);

        /// <summary>
        /// Create <see cref="FormattableString"/> with given parameters.
        /// </summary>
        /// <param name="parameters">An object contains parameters.</param>
        /// <returns>A <see cref="FormattableString"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
        public FormattableString ToFormattableString(object parameters)
            => FormattableStringFactory.Create(FormatString, createArguments(parameters));
    }
}
