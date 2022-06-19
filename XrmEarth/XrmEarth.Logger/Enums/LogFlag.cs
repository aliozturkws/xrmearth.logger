using System;

namespace XrmEarth.Logger.Enums
{
    [Flags]
    public enum LogFlag
    {
        Info = 1,
        Warning = 2,
        Error = 4,
        Object = 8,
        State = 16,
        Trace = 32
    }
}
