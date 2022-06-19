namespace XrmEarth.Logger.Connection
{
    public class FileConnectionComparer : IConnectionComparer
    {
        public bool Equals(IConnection x, IConnection y)
        {
            var connection1 = x as FileConnection;
            var connection2 = y as FileConnection;

            if (connection1 == null || connection2 == null)
            {
                if (connection1 == null && connection2 == null)
                {
                    return true;
                }
                return false;
            }

            return connection1.GetHashCode() == connection2.GetHashCode();
        }

        public int GetHashCode(IConnection obj)
        {
            return obj.GetHashCode();
        }
    }
}
