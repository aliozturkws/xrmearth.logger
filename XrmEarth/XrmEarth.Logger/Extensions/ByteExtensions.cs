using System;
using System.IO;
using System.Text;

namespace XrmEarth.Logger.Extensions
{
    public static class ByteExtensions
    {
        #region | Public Methods |

        public static string ToBase64String(this byte[] byteArray)
        {
            return Convert.ToBase64String(byteArray);
        }

        public static string GetString(this byte[] byteArray)
        {
            return Encoding.UTF8.GetString(byteArray);
        }

        /// <summary>
        /// Write all bytes to MemoryStream
        /// <example>
        /// <code>
        /// MemoryStream ms = File.ReadAllBytes(@"c:\test.txt").ToMemoryStream();
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static MemoryStream ToMemoryStream(this Byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            ms.Position = 0;
            return ms;
        } 

        #endregion
    }
}
