using System.Collections.Generic;

namespace XrmEarth.Logger.Data.Sql
{
    public class SPConfig
    {
        public string Name { get; set; }
        public IEnumerable<Parameter> Parameters { get; set; }
    }
}
