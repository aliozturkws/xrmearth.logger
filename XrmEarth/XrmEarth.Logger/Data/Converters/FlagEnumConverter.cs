using System;

namespace XrmEarth.Logger.Data.Converters
{
    public class FlagEnumConverter<T> : IValueConverter where T : struct 
    {
        public object Convert(object val)
        {
            if (val == null)
                return null;

            var parts = val.ToString().Split(' ');

            int enmVal;
            if (parts.Length == 2 && int.TryParse(parts[1], out enmVal))
                return (T)(object)enmVal;

            int? outputType = null;
            foreach (var part in parts)
            {
                T currOutputType;
                if (!Enum.TryParse(part, true, out currOutputType)) continue;
                var currVal = System.Convert.ToInt32(currOutputType);
                outputType = outputType.HasValue ? outputType.Value + currVal : currVal;
            }

            if (outputType.HasValue)
                return (T)(object)outputType;

            return null;
        }

        public object ConvertBack(object val)
        {
            return val.GetHashCode();
        }
    }
}
