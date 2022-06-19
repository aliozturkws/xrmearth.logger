using System;
using XrmEarth.Logger.Entity;

namespace XrmEarth.Samples.Data
{
    /// <summary>
    /// Çağırılan metodun ismi de loglanmak istenirse ICallerMember interface'i implemente edilmelidir. Bu sayede log yapısı çağıran metodu CallerMember özelliğine atayacaktır.
    /// </summary>
    public class DataClass : ICallerMember
    {
        public Guid ApplicationInstanceID { get; set; }

        public int ID { get; set; }
        public string ActionName { get; set; }
        public bool IsSuccess { get; set; }
        public string CallerMember { get; set; }
    }
}
