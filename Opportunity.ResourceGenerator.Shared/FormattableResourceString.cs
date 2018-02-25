using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Opportunity.ResourceGenerator
{
    /// <summary>
    /// Analyze interpolation string.
    /// </summary>
    [DebuggerDisplay(@"{FormatString}")]
    public sealed class FormattableResourceString
    {
        private static string analyze(string format, List<string> arguments)
        {
            var sb = new StringBuilder(format.Length);
            var inBrance = false;
            var currentArg = new StringBuilder(10);
            for (var i = 0; i < format.Length; i++)
            {
                switch (format[i])
                {
                case '{':
                    if (!inBrance)
                    {
                        inBrance = true;
                        sb.Append('{');
                    }
                    else if (currentArg.Length == 0)
                    {
                        inBrance = false;
                        sb.Append('{');
                    }
                    else
                    {
                        currentArg.Append('{');
                    }
                    break;
                case '}':
                    if (inBrance)
                    {
                        inBrance = false;
                        var arg = currentArg.ToString();
                        currentArg.Clear();
                        var argsp = arg.Split(new[] { ':' }, 2);
                        arg = argsp[0].Trim();
                        var argFormat = argsp.Length == 2 ? argsp[1] : null;
                        var argIndex = arguments.IndexOf(arg);
                        if (argIndex < 0)
                        {
                            argIndex = arguments.Count;
                            arguments.Add(arg);
                        }
                        sb.Append(argIndex);
                        if (argFormat != null)
                            sb.Append(':').Append(argFormat);
                        sb.Append('}');
                    }
                    else
                    {
                        try
                        {
                            if (format[i + 1] == '}')
                            {
                                sb.Append("}}");
                                i++;
                            }
                            else
                                throw new InvalidOperationException("Another '}' expected.");
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentException("Wrong format", nameof(format), ex);
                        }
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
                throw new ArgumentException("Wrong format", nameof(format));
            return sb.ToString();
        }

        /// <summary>
        /// Argument in <see cref="FormattableResourceString"/>
        /// </summary>
        [DebuggerDisplay(@"[{Name,nq}] = {Index}")]
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
            analyze(format, args);
            var oargs = args.OrderBy(s => s).ToList();
            FormatString = analyze(format, oargs);
            var argData = new Argument[args.Count];
            for (var i = 0; i < argData.Length; i++)
            {
                var arg = args[i];
                var index = oargs.IndexOf(arg);
                argData[i] = new Argument(arg, index);
            }
            this.Arguments = new ReadOnlyCollection<Argument>(argData);
        }

        /// <summary>
        /// Arguments of the <see cref="FormattableResourceString"/>, ordered by its appearance.
        /// </summary>
        public ReadOnlyCollection<Argument> Arguments { get; }
        /// <summary>
        /// Format string used for <see cref="string.Format(string, object[])"/>.
        /// </summary>
        public string FormatString { get; }

        /// <summary>
        /// Format string with given parameters.
        /// </summary>
        /// <param name="parameters">A dictionary of parameters.</param>
        /// <param name="provider">A format provider for formatting.</param>
        /// <returns>A formatted string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
        public string Format(IFormatProvider provider, IDictionary<string, object> parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            var args = new object[Arguments.Count];
            foreach (var item in this.Arguments)
            {
                parameters.TryGetValue(item.Name, out args[item.Index]);
            }
            return string.Format(provider, FormatString, args);
        }

        /// <summary>
        /// Format string with given parameters.
        /// </summary>
        /// <param name="parameters">A dictionary of parameters.</param>
        /// <param name="provider">A format provider for formatting.</param>
        /// <returns>A formatted string.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
        public string Format(IDictionary<string, object> parameters) => Format(null, parameters);
    }
}
