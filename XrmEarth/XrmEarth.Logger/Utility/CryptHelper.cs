using System;
using System.Text;

namespace XrmEarth.Logger.Utility
{
    public class CryptHelper
    {
        public static string Decrypt(string cryptedText)
        {
            if (string.IsNullOrEmpty(cryptedText))
                return string.Empty;

            return Encoding.UTF8.GetString(Convert.FromBase64String(cryptedText));
        }

        public static string Crypt(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        }
    }
}
