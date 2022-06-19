using System;

namespace XrmEarth.Logger.Target
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DefaultTargetAttribute : Attribute
    {
        public DefaultTargetAttribute(Type logTarget)
        {
            Target = logTarget;
        }

        public Type Target { get; private set; }

        public bool IsValid()
        {
            return Target != null && typeof (LogTarget).IsAssignableFrom(Target);
        }
    }
}
