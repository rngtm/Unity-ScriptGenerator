///-----------------------------------
/// ScriptGenerator
/// @ 2016 RNGTM(https://github.com/rngtm)
///-----------------------------------
namespace ScriptGenerator
{
    using System;
    using System.Collections.Generic;

    public static class LinqExtension
    {
        public static IEnumerable<T> ToEnumerable<T>(this Array array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                yield return (T)array.GetValue(i);
            }
        }
    }
}
