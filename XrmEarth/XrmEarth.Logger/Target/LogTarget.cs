using System;
using XrmEarth.Logger.Connection;
using XrmEarth.Logger.Logger;

namespace XrmEarth.Logger.Target
{
    [Serializable]
    public abstract class LogTarget
    {
        public string Key { get; set; }
        public IConnection Connection { get; set; }

        internal CoreLogger CreateLogger()
        {
            return OnCreateLogger();
        }

        protected abstract CoreLogger OnCreateLogger();

        public static T CastConnection<T>(IConnection connection) where T : IConnection
        {
            if (connection == null)
                return default(T);

            return (T)connection;
        }
    }
}
