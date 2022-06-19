namespace XrmEarth.Logger.Enums
{
    public enum LogType
    {
        /// <summary>
        /// Bilgi
        /// </summary>
        Info = 1,
        /// <summary>
        /// Uyarı
        /// </summary>
        Warning = 2,
        /// <summary>
        /// Hata
        /// </summary>
        Error = 4,
        /// <summary>
        /// Nesne
        /// </summary>
        Object = 8,
        /// <summary>
        /// Durum
        /// </summary>
        State = 16,
        /// <summary>
        /// Detay Bilgi
        /// </summary>
        Trace = 32
    }
}
