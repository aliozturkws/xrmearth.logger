using System;

namespace XrmEarth.Logger.Exceptions
{
    public class ApplicationNotRegisteredException : Exception
    {
        public ApplicationNotRegisteredException(string message)
            : base(message)
        {
            
        }
    }
}
