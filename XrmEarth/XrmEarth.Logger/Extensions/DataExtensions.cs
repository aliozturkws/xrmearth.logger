using System.Data;

namespace XrmEarth.Logger.Extensions
{
    public static class DataExtensions
    {
        #region | Public Methods |

        public static bool IsNullOrEmpty(this DataTable table)
        {
            return table == null || table.Rows.Count == 0;
        }

        public static bool HasRows(this DataTable value)
        {
            return value != null && value.Rows != null && value.Rows.Count > 0 ? true : false;
        }

        public static bool IsColumnExists(this IDataReader dataReader, string columnName)
        {
            bool result = false;

            dataReader.GetSchemaTable().DefaultView.RowFilter = string.Format("ColumnName= '{0}'", columnName);

            if (dataReader.GetSchemaTable().DefaultView.Count > 0)
            {
                result = true;
            }

            return result;
        }

        public static bool IsColumnExists(this DataTable datatable, string columnName)
        {
            return datatable.Columns.Contains(columnName);
        }

        #endregion
    }
}
