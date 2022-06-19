using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XrmEarth.Logger.Extensions
{
    public static class IEnumerableExtensions
    {
        #region | Public Methods |

        public static string ToString<T>(this IEnumerable<T> collection, Func<T, string> stringElement, string separator)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in collection)
            {
                sb.Append(stringElement(item));
                sb.Append(separator);
            }

            return sb.ToString(0, Math.Max(0, sb.Length - separator.Length));
        }

        public static string ToString<T>(this IEnumerable<T> self, string format)
        {
            return self.ToString(i => String.Format(format, i));
        }

        public static string ToString<T>(this IEnumerable<T> self, Func<T, object> function)
        {
            var result = new StringBuilder();

            foreach (var item in self)
            {
                result.Append(function(item));
            }

            return result.ToString();
        }

        public static string ToHtmlTable<T>(this IEnumerable<T> list, string tableSyle, string headerStyle, string rowStyle, string alternateRowStyle)
        {
            var result = new StringBuilder();

            if (String.IsNullOrEmpty(tableSyle))
            {
                result.Append("<table id=\"" + typeof(T).Name + "Table\">");
            }
            else
            {
                result.Append("<table id=\"" + typeof(T).Name + "Table\" class=\"" + tableSyle + "\">");
            }

            var propertyArray = typeof(T).GetProperties();

            foreach (var prop in propertyArray)
            {
                if (String.IsNullOrEmpty(headerStyle))
                {
                    result.AppendFormat("<th>{0}</th>", prop.Name);
                }
                else
                {
                    result.AppendFormat("<th class=\"{0}\">{1}</th>", headerStyle, prop.Name);
                }
            }

            for (int i = 0; i < list.Count(); i++)
            {
                if (!String.IsNullOrEmpty(rowStyle) && !String.IsNullOrEmpty(alternateRowStyle))
                {
                    result.AppendFormat("<tr class=\"{0}\">", i % 2 == 0 ? rowStyle : alternateRowStyle);
                }
                else
                {
                    result.AppendFormat("<tr>");
                }

                foreach (var prop in propertyArray)
                {
                    object value = prop.GetValue(list.ElementAt(i), null);
                    result.AppendFormat("<td>{0}</td>", value ?? String.Empty);
                }

                result.AppendLine("</tr>");
            }

            result.Append("</table>");
            return result.ToString();
        }

        /// <summary>
        /// Swap item to another place
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="value">Collection</param>
        /// <param name="IndexA">Index a</param>
        /// <param name="IndexB">Index b</param>
        /// <returns>New collection</returns>
        public static IList<T> Swap<T>(this IList<T> value, Int32 IndexA, Int32 IndexB)
        {
            T Temp = value[IndexA];
            value[IndexA] = value[IndexB];
            value[IndexB] = Temp;
            return value;
        }

        /// <summary>
        /// Swap item to the left
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="value">Collection</param>
        /// <param name="Index">Index</param>
        /// <returns>New collection</returns>
        public static IList<T> SwapLeft<T>(this IList<T> value, Int32 Index)
        {
            return value.Swap(Index, Index - 1);
        }

        /// <summary>
        /// Swap item to the right
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="value">Collection</param>
        /// <param name="Index">Index</param>
        /// <returns>New collection</returns>
        public static IList<T> SwapRight<T>(this IList<T> value, Int32 Index)
        {
            return value.Swap(Index, Index + 1);
        }

        public static IList<T> ActionAt<T>(this IList<T> value, Int32 Index, Action<T> ActionAt)
        {
            ActionAt(value[Index]);
            return value;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();

            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> value, Int32 Count)
        {
            return value.Shuffle().Take(Count);
        }

        public static T TakeRandom<T>(this IEnumerable<T> value)
        {
            return value.TakeRandom(1).Single();
        }

        public static List<T> InsertFirst<T>(this  List<T> lst, T Obj)
        {
            List<T> NewList = new List<T>();

            NewList.Add(Obj);
            NewList.AddRange(lst);

            return NewList;
        }

        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source)
        {
            if (!source.Any())
            {
                yield break;
            }

            Queue<T> items = new Queue<T>();
            items.Enqueue(source.First());

            foreach (T item in source.Skip(1))
            {
                yield return items.Dequeue();
                items.Enqueue(item);
            }
        }

        /// <summary>
        /// Random order
        /// </summary>
        /// <typeparam name="t"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static IEnumerable<t> Randomize<t>(this IEnumerable<t> target)
        {
            Random r = new Random();
            return target.OrderBy(x => (r.Next()));
        }

        /// <summary>
        /// Insert a constant item in-between each element of a list of items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static IEnumerable<T> Intersperse<T>(this IEnumerable<T> items, T separator)
        {
            var first = true;

            foreach (var item in items)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    yield return separator;
                }

                yield return item;
            }
        }

        /// <summary>
        /// Input: transpose [[1,2,3],[4,5,6],[7,8,9]]
        /// Output: [[1,4,7],[2,5,8],[3,6,9]]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Transpose<T>(this IEnumerable<IEnumerable<T>> values)
        {
            if (values.Count() == 0)
            {
                return values;
            }

            if (values.First().Count() == 0)
            {
                return Transpose(values.Skip(1));
            }

            var x = values.First().First();
            var xs = values.First().Skip(1);
            var xss = values.Skip(1);

            return
             new[] {new[] {x}
           .Concat(xss.Select(ht => ht.First()))}
               .Concat(new[] { xs }
               .Concat(xss.Select(ht => ht.Skip(1)))
               .Transpose());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
        {
            var r = new Random((int)DateTime.Now.Ticks);
            var shuffledList = list.Select(x => new { Number = r.Next(), Item = x }).OrderBy(x => x.Number).Select(x => x.Item);
            return shuffledList.ToList();
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || source.Count() == 0;
        }

        #endregion
    }
}
