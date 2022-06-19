using System;
using System.Collections.Generic;
using System.Linq;

namespace XrmEarth.Logger.Extensions
{
    public static class ICollectionExtensions
    {
        #region | Public Methods |

        public static string GetValue(this Dictionary<string, string> value, string key)
        {
            string result = string.Empty;

            if (value != null && !string.IsNullOrEmpty(key) && value.ContainsKey(key))
            {
                result = value[key].ToString();
            }

            return result;
        }

        public static IEnumerable<T> RemoveDuplicates<T>(this ICollection<T> list, Func<T, int> Predicate)
        {
            var dict = new Dictionary<int, T>();

            foreach (var item in list)
            {
                if (!dict.ContainsKey(Predicate(item)))
                {
                    dict.Add(Predicate(item), item);
                }
            }

            return dict.Values.AsEnumerable();
        }

        #endregion
    }
}
