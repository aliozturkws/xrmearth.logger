namespace XrmEarth.Logger.Initializer
{
    public interface IRequireInitialize<out T>
    {
        T Initialize();
    }
}
