using System.Collections.Generic;

namespace XrmEarth.Logger.Connection
{
    /// <summary>
    /// Sınıfın yazılmasının sebebi ileride Connection sınıflarının Core kütüphanesine taşınmasından sonra proje özelindeki işlemlerin Logging üzerinde kalması için.
    /// </summary>
    public interface IConnectionComparer : IEqualityComparer<IConnection>
    {
    }
}
