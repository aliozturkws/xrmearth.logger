using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XrmEarth.Logger.Renderer.Base
{
    public class DefaultKeyValueRenderer<T> : GenericRenderer<T>
    {
        public DefaultKeyValueRenderer(Dictionary<string, object> defaultValues)
        {
            _defaultValues = defaultValues;
        }

        public DefaultKeyValueRenderer()
        {
            _defaultValues = null;
        }

        private readonly Dictionary<string, object> _defaultValues;

        public override Action<object, Dictionary<string, object>> ValidateAction { get; set; }


        protected override Dictionary<string, object> OnRender(T value)
        {
            var t = typeof (T);
            var keyValueDictionary = _defaultValues == null ? new Dictionary<string, object>() : new Dictionary<string, object>(_defaultValues);

            var props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead);
            foreach (var kv in props.ToDictionary(p => p.Name, p => p.GetValue(value, null)))
            {
                keyValueDictionary.Add(kv.Key, kv.Value);
            }

            return keyValueDictionary;
        }
    }
}
