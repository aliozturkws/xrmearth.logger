namespace XrmEarth.Logger.Data.Converters
{
    public class EnumConverter<T> : IValueConverter
    {
        public object Convert(object val)
        {
            if (val == null)
                return default(T);

            return (T)(object)System.Convert.ToInt32(val);
        }

        public object ConvertBack(object val)
        {
            if (val == null)
                return default(T);

            return val.GetHashCode();
        }
    }
}
