﻿using System;
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
    [DebuggerDisplay(@"{FormatString}")]
    public sealed class FormattableResourceString
    {
        private static class DynamicCaller<T>
        {
            private readonly static CallSite<Func<CallSite, T, string, object>> indexCache
                = CallSite<Func<CallSite, T, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof(T), new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null) }));
            private readonly static Dictionary<string, CallSite<Func<CallSite, T, object>>> memberCache
                = new Dictionary<string, CallSite<Func<CallSite, T, object>>>();

            public static object Get(T param, string name)
            {
                try
                {
                    return indexCache.Target(indexCache, param, name);
                }
                catch { }
                if (memberCache.TryGetValue(name, out var callSite) && callSite != null)
                {
                    try
                    {
                        return callSite.Target(callSite, param);
                    }
                    catch { }
                }
                try
                {
                    callSite = CallSite<Func<CallSite, T, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, name, typeof(T), new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null) }));
                    var r = callSite.Target(callSite, param);
                    memberCache[name] = callSite;
                    return r;
                }
                catch { }
                return null;
            }
        }

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
        [DebuggerDisplay(@"[{Index}] = {Name}")]
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
        private readonly Argument[] arguments;
        /// <summary>
        /// Arguments of the <see cref="FormattableResourceString"/>, ordered by its appearance.
        /// </summary>
        public IReadOnlyList<Argument> Arguments => this.arguments;
        /// <summary>
        /// Format string used for <see cref="string.Format(string, object[])"/>.
        /// </summary>
        public string FormatString { get; }

        private object[] createArguments(IReadOnlyDictionary<string, object> parameters)
        {
            if (this.arguments.Length == 0)
                return Array.Empty<object>();
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var args = new object[Arguments.Count];
            foreach (var item in this.Arguments)
            {
                parameters.TryGetValue(item.Name, out args[item.Index]);
            }
            return args;
        }

        private object[] createArguments<T>(T parameters)
        {
            if (this.arguments.Length == 0)
                return Array.Empty<object>();
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var args = new object[Arguments.Count];
            foreach (var item in this.Arguments)
            {
                args[item.Index] = DynamicCaller<T>.Get(parameters, item.Name);
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
        public string Format<T>(IFormatProvider provider, T parameters)
            => string.Format(provider, FormatString, createArguments(parameters));

        /// <summary>
        /// Format string with given parameters.
        /// </summary>
        /// <param name="parameters">An object contains parameters.</param>
        /// <returns>A formatted string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
        public string Format<T>(T parameters) => Format(null, parameters);

        /// <summary>
        /// Create <see cref="FormattableString"/> with given parameters.
        /// </summary>
        /// <param name="parameters">An object contains parameters.</param>
        /// <returns>A <see cref="FormattableString"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
        public FormattableString ToFormattableString<T>(T parameters)
            => FormattableStringFactory.Create(FormatString, createArguments(parameters));
    }
}
