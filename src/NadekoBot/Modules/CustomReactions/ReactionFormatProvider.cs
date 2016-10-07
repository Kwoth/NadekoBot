using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NadekoBot.Modules.CustomReactions
{
    /// <summary>
    /// allows for formatting of the types
    /// </summary>
    public class ReactionFormatProvider : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ReactionFormatProvider))
                return this;
            return null;
        }
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (!Equals(formatProvider)) return null;
            //return null;
            if (string.IsNullOrWhiteSpace(format)) return "";
            //switch on type
            string result = string.Empty;
            TypeSwitch.Do(
                arg,
                TypeSwitch.Case<IMessage>((u) => result = fromMessage(format)),
                TypeSwitch.Case<System.Text.RegularExpressions.Match>(sup => result = fromMatch(format))


                );
            return string.Empty;
        }
        /// <summary>
        /// Regex match object
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private string fromMatch(string format)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// The message object
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private string fromMessage(string format)
        {
            throw new NotImplementedException();
        }
    }

    static class TypeSwitch
    {
        public class CaseInfo
        {
            public bool IsDefault { get; set; }
            public Type Target { get; set; }
            public Action<object> Action { get; set; }
        }

        public static void Do(object source, params CaseInfo[] cases)
        {
            var type = source.GetType();
            foreach (var entry in cases)
            {
                if (entry.IsDefault || entry.Target.IsAssignableFrom(type))
                {
                    entry.Action(source);
                    break;
                }
            }
        }

        public static CaseInfo Case<T>(Action action)
        {
            return new CaseInfo()
            {
                Action = x => action(),
                Target = typeof(T)
            };
        }

        public static CaseInfo Case<T>(Action<T> action)
        {
            return new CaseInfo()
            {
                Action = (x) => action((T)x),
                Target = typeof(T)
            };
        }

        public static CaseInfo Default(Action action)
        {
            return new CaseInfo()
            {
                Action = x => action(),
                IsDefault = true
            };
        }
    }
}
