using System;

namespace Windore.Settings.Base
{
    // This allows list containing different generic ConvertFunctions
    internal interface IConvertFunction 
    {
        Type Type { get; }
    }

    public class ConvertFunction<T> : IConvertFunction
    {
        private Func<T, string> toString;
        private Func<string, T> fromString;
        public Type Type => typeof(T);

        public ConvertFunction(Func<T, string> toStringFunc, Func<string, T> fromStringFunc) 
        {
            toString = toStringFunc;
            fromString = fromStringFunc;
        }

        public string ConvertToString(T obj) => toString(obj);
        public T ConvertFromString(string s) => fromString(s);
    }
}