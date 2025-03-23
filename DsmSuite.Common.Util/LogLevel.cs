using System;

namespace DsmSuite.Common.Util
{
    [Serializable]
    public enum LogLevel
    {
        None,
        User,
        Error,
        Warning,
        Info,
        Data,
        All
    }
}
