using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace XrmEarth.Logger.Extensions
{
    //public static class StringExtensions
    //{
    //    #region | Private Definitions |

    //    public static readonly char DefaultMaskCharacter = '*';

    //    #endregion

    //    #region | Public Methods |

    //    public static bool IsEmpty(this string stringValue)
    //    {
    //        return (stringValue == string.Empty);
    //    }

    //    public static bool IsNull(this string stringValue)
    //    {
    //        return (stringValue == null);
    //    }

    //    public static bool IsNullOrEmpty(this string stringValue)
    //    {
    //        return string.IsNullOrEmpty(stringValue);
    //    }

    //    public static byte[] FromBase64String(this string stringValue)
    //    {
    //        return Convert.FromBase64String(stringValue);
    //    }

    //    public static byte[] GetBytes(this string stringValue)
    //    {
    //        return Encoding.UTF8.GetBytes(stringValue);
    //    }

    //    public static byte[] GetBytes(this string stringValue, Encoding encoding)
    //    {
    //        return encoding.GetBytes(stringValue);
    //    }

    //    public static string ToBase64String(this string stringValue)
    //    {
    //        return Convert.ToBase64String(Encoding.UTF8.GetBytes(stringValue));
    //    }

    //    public static string ToBase64String(this string stringValue, Encoding encoding)
    //    {
    //        return Convert.ToBase64String(encoding.GetBytes(stringValue));
    //    }

    //    public static string GetStringFromBase64(this string stringValue)
    //    {
    //        return Encoding.UTF8.GetString(Convert.FromBase64String(stringValue));
    //    }

    //    public static string GetStringFromBase64(this string stringValue, Encoding encoding)
    //    {
    //        return encoding.GetString(Convert.FromBase64String(stringValue));
    //    }

    //    public static DateTime ToDateTime(this string stringValue)
    //    {
    //        return DateTime.Parse(stringValue);
    //    }

    //    public static DateTime ToDateTime(this string stringValue, CultureInfo culture)
    //    {
    //        return DateTime.Parse(stringValue, culture.DateTimeFormat);
    //    }

    //    public static DateTime ToDateTime(this string stringValue, IFormatProvider formatProvider)
    //    {
    //        return DateTime.Parse(stringValue, formatProvider);
    //    }

    //    public static string ToMD5String(this string stringValue)
    //    {
    //        MD5 md5Hash = MD5.Create();

    //        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(stringValue));

    //        StringBuilder md5HexadecimalString = new StringBuilder();

    //        for (int i = 0; i < data.Length; i++)
    //        {
    //            md5HexadecimalString.Append(data[i].ToString("x2"));
    //        }

    //        return md5HexadecimalString.ToString();
    //    }

    //    public static bool VerifyMD5String(this string stringValue, string md5String)
    //    {
    //        return stringValue.ToMD5String().CompareTo(md5String) == 0;
    //    }

    //    public static T ConvertTo<T>(this string stringValue) where T : struct
    //    {
    //        return (T)Convert.ChangeType(stringValue, typeof(T));
    //    }

    //    public static T ConvertTo<T>(this string stringValue, IFormatProvider formatProvider) where T : struct
    //    {
    //        return (T)Convert.ChangeType(stringValue, typeof(T), formatProvider);
    //    }

    //    public static bool IsGuid(this string value)
    //    {
    //        Exceptions.ExceptionThrow.IfNullOrEmpty(value, "Value");

    //        Regex format = new Regex("^[A-Fa-f0-9]{32}$|" + "^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|" + "^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$");
    //        Match match = format.Match(value);

    //        return match.Success;
    //    }

    //    public static Guid ToGuid(this string value)
    //    {
    //        Guid result = Guid.Empty;

    //        value = value.Replace("{", "").Replace("}", "");

    //        if (value.IsGuid())
    //        {
    //            result = new Guid(value.Trim());
    //        }

    //        return result;
    //    }

    //    public static string ReplaceTurkishChars(this string value)
    //    {
    //        Exceptions.ExceptionThrow.IfNullOrEmpty(value, "Value");

    //        string result = value;
    //        string[] charTR = new string[] { "ı", "İ", "ğ", "Ğ", "ü", "Ü", "ş", "Ş", "ö", "Ö", "ç", "Ç" };
    //        string[] charEN = new string[] { "i", "I", "g", "G", "u", "U", "s", "S", "o", "O", "c", "c" };

    //        for (int i = 0; i < charTR.Length; i++)
    //        {
    //            result = result.Replace(charTR[i], charEN[i]);
    //        }

    //        return result;
    //    }

    //    public static string ReplaceChars(this string value, string[] originalChars, string[] replacedChars)
    //    {
    //        Exceptions.ExceptionThrow.IfNullOrEmpty(value, "Value");
    //        Exceptions.ExceptionThrow.IfNullOrEmpty(originalChars, "originalChars");
    //        Exceptions.ExceptionThrow.IfNullOrEmpty(replacedChars, "replacedChars");

    //        string result = value;

    //        if (originalChars.Length != replacedChars.Length)
    //        {
    //            throw new ArgumentOutOfRangeException("Tüm Array parametrelerinin uzunluk (Array.Length) değerleri aynı olmalı.");
    //        }

    //        for (int i = 0; i < originalChars.Length; i++)
    //        {
    //            result = result.Replace(originalChars[i].ToString(), replacedChars[i].ToString());
    //        }

    //        return result;
    //    }

    //    public static string Right(this string value, int length)
    //    {
    //        return value != null && value.Length > length ? value.Substring(value.Length - length) : value;
    //    }

    //    public static string Left(this string value, int length)
    //    {
    //        return value != null && value.Length > length ? value.Substring(0, length) : value;
    //    }

    //    public static string ToHex(this string value)
    //    {
    //        string result = string.Empty;

    //        foreach (char c in value)
    //        {
    //            int tmp = c;
    //            result += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
    //        }

    //        return result;
    //    }

    //    public static string ToProperCase(this string value)
    //    {
    //        System.Globalization.CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
    //        System.Globalization.TextInfo textInfo = cultureInfo.TextInfo;
    //        return textInfo.ToTitleCase(value);
    //    }

    //    public static bool IsValidEmailAddress(this string value)
    //    {
    //        Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
    //        return regex.IsMatch(value);
    //    }

    //    public static bool IsValidUrl(this string text)
    //    {
    //        Regex rx = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
    //        return rx.IsMatch(text);
    //    }

    //    public static bool IsNumeric(this string value)
    //    {
    //        Regex regex = new Regex(@"[0-9]");
    //        return regex.IsMatch(value);
    //    }

    //    public static bool IsLengthAtLeast(this string value, int length)
    //    {
    //        Exceptions.ExceptionThrow.IfNegative(length, "length");
    //        return value != null ? value.Length >= length : false;
    //    }

    //    public static string RemoveHtml(this string value)
    //    {
    //        Exceptions.ExceptionThrow.IfNullOrEmpty(value, "Value");
    //        return Regex.Replace(value, @"<(.|\n)*?>", string.Empty, RegexOptions.Multiline | RegexOptions.IgnoreCase);
    //    }

    //    public static string Reverse(this string value)
    //    {
    //        Exceptions.ExceptionThrow.IfNullOrEmpty(value, "Value");
    //        StringBuilder sb = new StringBuilder();

    //        for (int i = value.Length - 1; i >= 0; i--)
    //        {
    //            sb.Append(value.Substring(i, 1));
    //        }

    //        return sb.ToString();
    //    }

    //    public static string RemoveNumerics(this string value)
    //    {
    //        return Regex.Replace(value, @"[\d-]", string.Empty);
    //    }

    //    public static string GetNumberics(this string value)
    //    {
    //        return Regex.Match(value, @"\d+").Value;
    //    }

    //    public static string Truncate(this string value, int maxLength, string suffix = "....")
    //    {
    //        string truncatedString = value;
    //        int strLength = maxLength - suffix.Length;

    //        if (maxLength <= 0)
    //        {
    //            return truncatedString;
    //        }

    //        if (strLength <= 0)
    //        {
    //            return truncatedString;
    //        }

    //        if (value == null || value.Length <= maxLength)
    //        {
    //            return truncatedString;
    //        }

    //        truncatedString = value.Substring(0, strLength);
    //        truncatedString = truncatedString.TrimEnd();
    //        truncatedString += suffix;

    //        return truncatedString;
    //    }

    //    public static bool In(this string value, params string[] stringValues)
    //    {
    //        Exceptions.ExceptionThrow.IfNullOrEmpty(value, "Value");

    //        foreach (string otherValue in stringValues)
    //        {
    //            if (string.Compare(value, otherValue) == 0)
    //            {
    //                return true;
    //            }
    //        }

    //        return false;
    //    }

    //    #endregion
    //}
}
