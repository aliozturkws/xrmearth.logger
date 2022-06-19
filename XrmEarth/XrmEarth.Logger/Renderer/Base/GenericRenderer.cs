using System.Collections.Generic;
using XrmEarth.Logger.Common;
using XrmEarth.Logger.Exceptions;

namespace XrmEarth.Logger.Renderer.Base
{
    public abstract class GenericRenderer<T> : BaseRenderer
    {
        protected override Dictionary<string, object> OnRenderObject(object value)
        {
            if (CheckType<T>(value))
                return OnRender((T) value);

            if (value != null && (value.GetType()).IsAssignableFrom(typeof (IValueContainer)))
                if (CheckType<T>(((IValueContainer) value).Value))
                    return OnRender((T) ((IValueContainer) value).Value);
            
            throw new InvalidTypeException(string.Format("'{0}' tipi için '{1}' tipindeki 'IRenderer' sınıfı kullanılamaz.", value == null ? "Null" : value.GetType().Name, typeof(T).Name));
        }

        protected abstract Dictionary<string, object> OnRender(T value);

        protected bool CheckType<T>(object value)
        {
            if (value == null)
                return false;

            var oType = value.GetType();
            return oType.IsAssignableFrom(typeof (T));
        }
    }
}
