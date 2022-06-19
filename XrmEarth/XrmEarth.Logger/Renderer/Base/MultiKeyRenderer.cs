using System;
using System.Collections.Generic;
using System.Linq;
using XrmEarth.Logger.Common;
using XrmEarth.Logger.Exceptions;
using XrmEarth.Logger.Renderer.Content;
using XrmEarth.Logger.Renderer.Mssql;
using XrmEarth.Logger.Renderer.Smtp;

namespace XrmEarth.Logger.Renderer.Base
{
    public class MultiKeyRenderer<TKey, TRenderer> : BaseRenderer, IMssqlRenderer, ISmtpRenderer, IContentRenderer
        where TRenderer : IRenderer
    {
        public MultiKeyRenderer(IEnumerable<KeyValuePair<TKey, TRenderer>> typeRenderers = null)
        {
            _resolvers = new Dictionary<TKey, TRenderer>();
            if (typeRenderers != null)
            {
                foreach (var typeRenderer in typeRenderers)
                {
                    _resolvers[typeRenderer.Key] = typeRenderer.Value;
                }
            }

            _validateAction = (value, keyValuePairs) =>
            {
                var renderer = RendererSelectionAction.Invoke(GetValueContainer(value).Key);
                
                if (renderer.ValidateAction != null)
                    renderer.ValidateAction.Invoke(value, keyValuePairs);
            };

            _defaultRendererSelectionAction = key =>
            {
                if (!_resolvers.ContainsKey(key))
                    throw new RendererNotFoundException(string.Format("'{0}' anahtarına ait 'IRenderer' nesnesi bulunamadı.", key));

                return _resolvers[key];
            };
            _rendererSelectionAction = _defaultRendererSelectionAction;
        }

        private readonly Dictionary<TKey, TRenderer> _resolvers;
        private readonly Func<TKey, TRenderer> _defaultRendererSelectionAction;

        private readonly Action<object, Dictionary<string, object>> _validateAction;
        public override Action<object, Dictionary<string, object>> ValidateAction
        {
            get { return _validateAction; }
            set { }
        }

        private Func<TKey, TRenderer> _rendererSelectionAction;
        public Func<TKey, TRenderer> RendererSelectionAction
        {
            get
            {
                return _rendererSelectionAction;
            }
            set
            {
                if (value == null)
                    _rendererSelectionAction = _defaultRendererSelectionAction;

                _rendererSelectionAction = value;
            }
        }


        public void Register(TKey key, TRenderer renderer)
        {
            _resolvers[key] = renderer;
        }

        public bool Unregister(TKey key)
        {
            if (!_resolvers.ContainsKey(key))
                return false;

            _resolvers.Remove(key);
            return true;
        }

        public bool Contains(TKey key)
        {
            return _resolvers.ContainsKey(key);
        }

        internal TKey[] GetRegisteredKeys()
        {
            return _resolvers.Keys.ToArray();
        }


        protected override Dictionary<string, object> OnRenderObject(object value)
        {
            var keyValueContainer = GetValueContainer(value);
            var renderer = RendererSelectionAction.Invoke(keyValueContainer.Key);
            return renderer.RenderObject(keyValueContainer.Value);
        }

        public IKeyValueContainer<TKey> GetValueContainer(object value)
        {
            var keyValueContainer = value as IKeyValueContainer<TKey>;
            if (keyValueContainer == null)
                throw new InvalidTypeException(string.Format("'{0}' tipi render edilmek için geçersizdi. Nesne '{1}' tipinden türemiş olmalı.", value == null ? "Null" : value.GetType().Name, typeof(IKeyValueContainer<TKey>).Name));

            return keyValueContainer;
        }

        public Func<object, string> GetSubject { get; set; }
        public Func<object, string> GetBody { get; set; }
        public Func<object, string> GetContent { get; set; }
    }
}
