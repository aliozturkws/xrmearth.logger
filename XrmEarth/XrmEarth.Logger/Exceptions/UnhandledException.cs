using System;
using System.Text;
using XrmEarth.Logger.Enums;

namespace XrmEarth.Logger.Exceptions
{
    public class UnhandledException : Exception
    {
        public UnhandledException(bool isTerminating = false, UnhandledExceptionType type = UnhandledExceptionType.Internal)
        {
            IsTerminating = isTerminating;
            Type = type;
        }

        public UnhandledException(string message, bool isTerminating = false, UnhandledExceptionType type = UnhandledExceptionType.Internal)
            : base(message)
        {
            IsTerminating = isTerminating;
            Type = type;
        }

        public UnhandledException(string message, Exception innerException, bool isTerminating = false, UnhandledExceptionType type = UnhandledExceptionType.Internal)
            : base(message, innerException)
        {
            IsTerminating = isTerminating;
            Type = type;
        }

        public UnhandledExceptionType Type { get; set; }
        public bool IsTerminating { get; private set; }
        public object Object { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder()
                .Append("Type: ").Append(Type).AppendLine()
                .Append("IsTerminating: ").Append(IsTerminating ? "Yes" : "No").AppendLine()
                .Append("Have Object: ").Append(Object != null ? "Yes" : "No").AppendLine();

            if (Object != null)
            {
                try
                {
                    builder.Append("Object: ").Append(JsonSerializerUtil.Serialize(Object)).AppendLine();
                }
                catch (Exception e)
                {
                    builder.Append("Object serialization exception -> ").Append(e).AppendLine();
                }
            }

            builder.Append(base.ToString());
            return builder.ToString();
        }
    }
}
