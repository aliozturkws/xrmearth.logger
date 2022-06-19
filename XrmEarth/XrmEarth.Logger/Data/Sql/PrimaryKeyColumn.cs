namespace XrmEarth.Logger.Data.Sql
{
    public class PrimaryKeyColumn : Column
    {
        public bool AutoIncrement { get; set; }

        public override string CreateQuery()
        {
            return base.CreateQuery() + " " + (AutoIncrement ? "IDENTITY(1,1)" : string.Empty) + "PRIMARY KEY";
        }
    }
}
