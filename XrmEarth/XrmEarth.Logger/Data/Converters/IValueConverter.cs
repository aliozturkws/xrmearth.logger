namespace XrmEarth.Logger.Data.Converters
{
    public interface IValueConverter
    {
        object Convert(object value);
        object ConvertBack(object value);
    }
}
