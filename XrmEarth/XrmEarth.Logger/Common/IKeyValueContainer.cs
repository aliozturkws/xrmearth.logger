namespace XrmEarth.Logger.Common
{
    public interface IKeyValueContainer<T> : IValueContainer
    {
        T Key { get; set; }
    }
}
