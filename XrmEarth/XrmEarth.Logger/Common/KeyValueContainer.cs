namespace XrmEarth.Logger.Common
{
    public class KeyValueContainer<TKey> : IKeyValueContainer<TKey>
    {
        public KeyValueContainer(TKey key, object value)
        {
            Key = key;
            Value = value;
        }

        public object Value { get; private set; }
        public TKey Key { get; set; }
    }
}
