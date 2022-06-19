using System;
using System.ComponentModel;

namespace XrmEarth.Logger.Extensions
{
    public static class EnumExtensions
    {
        #region | Public Methods |

        public static string GetDescription(this Enum value)
        {
            string result = string.Empty;
            string name = Enum.GetName(value.GetType(), value);

            if (name != null)
            {
                var field = value.GetType().GetField(name);

                if (field != null)
                {
                    var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

                    if (attr != null)
                    {
                        result = attr.Description;
                    }
                }
            }

            return result;
        } 

        #endregion
    }
}
