using System.Data;

namespace XrmEarth.Logger.Data.Sql
{
    public class Column
    {
        public string Name { get; set; }
        public SqlDbType SqlDbType { get; set; }
        public bool IsNullable { get; set; }
        public int Length { get; set; }
        public string Range { get; set; }

        public virtual string CreateQuery()
        {
            return Name + " " + GetString(SqlDbType) + (IsNullable ? " NULL" : " NOT NULL");
        }

        protected string GetString(SqlDbType sqlDbType)
        {
            var text = sqlDbType.ToString();
            if (sqlDbType == SqlDbType.NChar)
            {
                text += " (4000)";
            }
            else if (sqlDbType == SqlDbType.Char)
            {
                text += " (8000)";
            }
            else if (sqlDbType == SqlDbType.NVarChar || sqlDbType == SqlDbType.VarChar)
            {
                text += " (MAX)";
            }
            return text;
        }
    }
}
