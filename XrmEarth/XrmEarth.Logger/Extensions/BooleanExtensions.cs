using System;
using System.Text;

namespace XrmEarth.Logger.Extensions
{
    public static class BooleanExtensions
    {
        #region | Enums |

        public enum BooleanFormat
        {
            OneZero,
            YN,
            YesNo,
            TF,
            TrueFalse,
            PassFail,
            YepNope
        }

        #endregion

        #region | Public Methods |

        public static string ToString(this bool value, string passValue, string failValue)
        {
            return value ? passValue : failValue;
        }

        public static string ToString(this bool value, BooleanFormat booleanFormat)
        {
            string booleanFormatString = Enum.GetName(booleanFormat.GetType(), booleanFormat);
            return ParseBooleanString(value, booleanFormatString);
        }

        #endregion

        #region | Private Methods |

        private static string ParseBooleanString(bool value, string booleanFormatString)
        {
            StringBuilder trueString = new StringBuilder();
            StringBuilder falseString = new StringBuilder();

            int charCount = booleanFormatString.Length;

            bool isTrueString = true;

            for (int i = 0; i != charCount; i++)
            {
                if (char.IsUpper(booleanFormatString[i]) && i != 0)
                {
                    isTrueString = false;
                }

                if (isTrueString)
                {
                    trueString.Append(booleanFormatString[i]);
                }
                else
                {
                    falseString.Append(booleanFormatString[i]);
                }
            }

            return (value == true ? trueString.ToString() : falseString.ToString());
        }

        #endregion
    }
}
