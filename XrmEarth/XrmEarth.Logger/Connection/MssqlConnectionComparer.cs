namespace XrmEarth.Logger.Connection
{
    public class MssqlConnectionComparer : IConnectionComparer
    {
        public bool Equals(IConnection x, IConnection y)
        {
            var connection1 = x as MssqlConnection;
            var connection2 = y as MssqlConnection;

            if (connection1 == null || connection2 == null)
            {
                if (connection1 == null && connection2 == null)
                {
                    return true;
                }
                return false;
            }

            var sv1 = connection1.Server;
            var sv2 = connection2.Server;
            if ((string.IsNullOrWhiteSpace(sv1) || string.IsNullOrWhiteSpace(sv2)))
            {
                return false;
            }

            if (string.Compare(sv1, sv2, true, ApplicationShared.CultureInfo) != 0)
            {
                return false;
            }

            var db1 = connection1.Database;
            var db2 = connection2.Database;
            if ((string.IsNullOrWhiteSpace(db1) || string.IsNullOrWhiteSpace(db2)))
            {
                return false;
            }

            if (string.Compare(db1, db2, true, ApplicationShared.CultureInfo) != 0)
            {
                return false;
            }

            return true;
        }

        public int GetHashCode(IConnection obj)
        {
            var connection = obj as MssqlConnection;
            return connection != null ? (connection.Server != null ? connection.Server.GetHashCode() : 0) ^ (connection.Database != null ? connection.Database.GetHashCode() : 0) : 0; 
        }
    }
}
