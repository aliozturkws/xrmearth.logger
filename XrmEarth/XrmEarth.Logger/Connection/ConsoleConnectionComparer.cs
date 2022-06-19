namespace XrmEarth.Logger.Connection
{
    public class ConsoleConnectionComparer : IConnectionComparer
    {
        public bool Equals(IConnection x, IConnection y)
        {
            var connection1 = x as ConsoleConnection;
            var connection2 = y as ConsoleConnection;

            if (connection1 == null || connection2 == null)
            {
                if (connection1 == null && connection2 == null)
                {
                    return true;
                }
                return false;
            }

            return true;
        }

        public int GetHashCode(IConnection obj)
        {
            var connection = obj as ConsoleConnection;
            return connection != null ? typeof(ConsoleConnection).GetHashCode() : 0;
        }
    }
}
