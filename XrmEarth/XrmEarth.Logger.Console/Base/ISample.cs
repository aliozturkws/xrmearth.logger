using System;

namespace XrmEarth.Logger.Console.Base
{
    public interface ISample
    {
        IOutput Output { get; }

        void Run();
    }

    public abstract class BaseSample : ISample
    {
        protected BaseSample()
        {
            Output = new ConsoleOutput();
        }

        public IOutput Output { get; protected set; }

        public void Run()
        {
            var name = GetType().Name;
            Output.WriteLine(name + " started.");

            try
            {
                OnRun();
            }
            catch (Exception ex)
            {
                Output.WriteLine(ex.ToString());
            }

            Output.WriteLine(name + " completed.");
        }

        protected abstract void OnRun();
    }

    public interface IOutput
    {
        void Write(string message);
        void WriteLine(string message);
    }

    public class ConsoleOutput : IOutput
    {
        public void Write(string message)
        {
            System.Console.Write(message);
        }

        public void WriteLine(string message)
        {
            System.Console.WriteLine(message);
        }
    }
}
