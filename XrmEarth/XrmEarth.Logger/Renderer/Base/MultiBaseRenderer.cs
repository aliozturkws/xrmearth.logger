using System;
using System.Collections.Generic;
using System.Linq;
using XrmEarth.Logger.Common;
using XrmEarth.Logger.Exceptions;

namespace XrmEarth.Logger.Renderer.Base
{
    public class MultiBaseRenderer<TRenderer> : BaseRenderer where TRenderer : IRenderer
    {
        public MultiBaseRenderer(IEnumerable<KeyValuePair<Type, TRenderer>> typeRenderers = null)
        {
            _resolvers = new Dictionary<Type, TRenderer>();
            if (typeRenderers != null)
            {
                foreach (var typeRenderer in typeRenderers)
                {
                    _resolvers[typeRenderer.Key] = typeRenderer.Value;
                }
            }

            _validateAction = (value, keyValuePairs) =>
            {
                var renderer = GetRenderer(value);
                
                if (renderer.ValidateAction != null)
                    renderer.ValidateAction.Invoke(value, keyValuePairs);
            };
        }

        private readonly Dictionary<Type, TRenderer> _resolvers;

        private readonly Action<object, Dictionary<string, object>> _validateAction;
        public override Action<object, Dictionary<string, object>> ValidateAction
        {
            get { return _validateAction; }
            set { }
        }


        public void Register<T>(TRenderer renderer)
        {
            _resolvers[typeof(T)] = renderer;
        }

        public bool Unregister<T>()
        {
            var t = typeof(T);
            if (!_resolvers.ContainsKey(t))
                return false;

            _resolvers.Remove(t);
            return true;
        }

        public TRenderer GetRegisteredRenderer(Type type)
        {
            if (Contains(type))
                return _resolvers[type];

            return default(TRenderer);
        }

        public bool Contains(Type type)
        {
            return _resolvers.ContainsKey(type);
        }

        internal Type[] GetRegisteredTypes()
        {
            return _resolvers.Keys.ToArray();
        }


        protected override Dictionary<string, object> OnRenderObject(object value)
        {
            var renderer = GetRenderer(value);
            return renderer.RenderObject(value);
        }

        private TRenderer GetRenderer(object value)
        {
            var oType = GetType(value);
            if (!_resolvers.ContainsKey(oType))
                throw new RendererNotFoundException(string.Format("Could not find object 'IRenderer' of type '{0}'.", oType));

            return _resolvers[oType];
        }

        private Type GetType(object value)
        {
            if (value == null) return null;

            if (value is IValueContainer)
                value = ((IValueContainer)value).Value;

            if (value == null) return null;

            return value.GetType();
        }
    }
}
