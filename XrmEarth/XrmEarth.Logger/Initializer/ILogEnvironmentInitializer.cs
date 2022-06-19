namespace XrmEarth.Logger.Initializer
{
    /// <summary>
    /// Ortam hazırlayıcı.
    /// <para></para>
    /// Bazı loglayıcılar için ortam değişkenleri gerekebilir, bunları oluşturma veya düzenleme işlemlerini yürütür.
    /// </summary>
    public interface ILogEnvironmentInitializer
    {
        string InitializeEnvironment();
    }
}
