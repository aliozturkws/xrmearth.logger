namespace XrmEarth.Logger.Data.Sql
{
    public class TableConfig
    {
        public PrimaryKeyColumn PrimaryKey { get; set; }
        public ForeignKeyColumn[] ForeignKeys { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
    }
}
