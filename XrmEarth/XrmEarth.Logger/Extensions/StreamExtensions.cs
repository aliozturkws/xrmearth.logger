using System;
using System.IO;

namespace XrmEarth.Logger.Extensions
{
    public static class StreamExtensions
    {
        #region | Public Methods |

        public static byte[] ToByteArray(this Stream value)
        {
            value.Position = 0;
            byte[] result = new byte[value.Length];

            for (int totalBytesCopied = 0; totalBytesCopied < value.Length; )
            {
                totalBytesCopied += value.Read(result, totalBytesCopied, Convert.ToInt32(value.Length) - totalBytesCopied);
            }

            return result;
        }

        #endregion
    }
}
