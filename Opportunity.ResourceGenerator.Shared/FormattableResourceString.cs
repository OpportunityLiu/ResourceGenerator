using System;
using System.Collections.Generic;
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
            analyze(format, this.arguments);
            this.orderedArguments = this.arguments.OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToList();
            FormatString = analyze(format, this.orderedArguments);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<string> arguments = new List<string>();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<string> orderedArguments;
        /// <summary>
        /// Arguments of the <see cref="FormattableResourceString"/>, ordered by its appearance.
        /// </summary>
        public IReadOnlyList<string> Arguments => this.arguments;
        /// <summary>
        /// Arguments of the <see cref="FormattableResourceString"/>, ordered by <see cref="StringComparer.OrdinalIgnoreCase"/>.
        /// This is the actual order of indices in <see cref="FormatString"/>.
        /// </summary>
        public IReadOnlyList<string> OrderedArguments => this.orderedArguments;
        /// <summary>
        /// Format string used for <see cref="string.Format(string, object[])"/>.
        /// </summary>
        public string FormatString { get; }
    }
}
