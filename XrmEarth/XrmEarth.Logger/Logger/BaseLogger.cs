using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using XrmEarth.Logger.Connection;
using XrmEarth.Logger.Entity;
using XrmEarth.Logger.Renderer;
using XrmEarth.Logger.Renderer.Base;

namespace XrmEarth.Logger.Logger
{
    public abstract class BaseLogger<T, T1> : CoreLogger
        where T : IConnection
        where T1 : BaseRenderer, IRenderer
    {
        protected BaseLogger(T connection, T1 renderer, bool followApplication, bool isLazyFollowApplication = true) : base(followApplication)
        {
            Connection = connection;
            Renderer = renderer;
            if (!isLazyFollowApplication)
            {
                _isInitialized = true;
                Initialize(Connection);
            }
            // Initialize(Connection); -> Go To Lazy _isInitialized;
        }

        private bool _isInitialized;

        public T Connection { get; private set; }
        public T1 Renderer { get; private set; }

        public override void Push(object value, [CallerMemberName]string memberName = "")
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                Initialize(Connection);
            }

            try
            {
                TryBind(value, memberName);
                var kv = Renderer.RenderObject(value);
                OnPush(kv);

                if (Renderer.ValidateAction != null)
                    Renderer.ValidateAction.Invoke(value, kv);
            }
            catch (Exception ex)
            {
                HandleException(ex, value);
            }
        }

        private void TryBind(object value, string memberName)
        {
            var al = value as ApplicationLog; 
            if (al != null)
            {
                al.ID = Guid.NewGuid();
                al.ApplicationInstanceID = CurrentContainer != null && CurrentContainer.HasInjected ? CurrentContainer.ApplicationInstance.ID : Guid.Empty;
            }

            var callerMember = value as ICallerMember;
            if (callerMember != null)
            {
                callerMember.CallerMember = memberName;
            }
        }

        protected abstract void OnPush(Dictionary<string, object> keyValuesDictionary);
    }
}
