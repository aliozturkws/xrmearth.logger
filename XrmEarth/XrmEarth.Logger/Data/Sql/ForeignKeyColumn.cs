namespace XrmEarth.Logger.Data.Sql
{
    public class ForeignKeyColumn : Column
    {
        public string ReferenceTableName { get; set; }
        public string ReferenceColumnName { get; set; }

        public override string CreateQuery()
        {
            return base.CreateQuery() + " FOREIGN KEY REFERENCES " + ReferenceTableName + "(" + ReferenceColumnName + ")";
        }
    }
}
