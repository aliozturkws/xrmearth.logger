using System;
using XrmEarth.Logger.Connection;
using XrmEarth.Logger.Entity;

namespace XrmEarth.Logger
{
    public class ApplicationContainer
    {
        public IConnection SourceConnection { get; set; }

        private readonly Guid ApplicationID = Guid.NewGuid();
        private readonly Guid ApplicationInstanceID = Guid.NewGuid();

        public bool HasInjected { get { return Application != null; } }

        private Application _application;
        public Application Application
        {
            get
            {
                if (_application == null && ApplicationShared.Summary != null)
                {
                    _application = new Application
                    {
                        ID = ApplicationID,
                        AssemblyGuid = ApplicationShared.Summary.Guid,
                        AssemblyVersion = ApplicationShared.Summary.Version.ToString(),
                        Name = ApplicationShared.Summary.Name,
                        Namespace = ApplicationShared.Summary.SolutionName
                    };
                }
                return _application;
            }
        }

        private ApplicationInstance _applicationInstance;
        public ApplicationInstance ApplicationInstance
        {
            get
            {
                if (_applicationInstance == null && ApplicationShared.Summary != null)
                {
                    _applicationInstance = new ApplicationInstance
                    {
                        ID = ApplicationInstanceID,
                        ApplicationID = Application.ID,
                        StartAt = ApplicationShared.SafeStartTime,
                        Parameters = string.Join(" ", Environment.GetCommandLineArgs()),
                        Path = ApplicationShared.Summary.ExecutablePath
                    };
                }
                return _applicationInstance;
            }
        }

        public override bool Equals(object obj)
        {
            var ac = obj as ApplicationContainer;
            if (ac == null)
                return false;

            if (ac.SourceConnection == null || SourceConnection == null)
                return false;

            var comp = ApplicationShared.GetConnectionComparer(SourceConnection);
            if (comp != null)
            {
                return comp.Equals(SourceConnection, ac.SourceConnection);
            }
            return false;
        }

        public override int GetHashCode()
        {
            var comp = ApplicationShared.GetConnectionComparer(SourceConnection);
            if (comp != null)
            {
                return comp.GetHashCode(SourceConnection);
            }
            return 0;
        }


        internal void OnApplicationClosing()
        {
            var sum = new InstanceSummary();
            ApplicationClosing(this, sum);

            ApplicationInstance.FinishAt = DateTime.Now;
            ApplicationInstance.Summary = sum.Summary;
            ApplicationInstance.Result = sum.Result;
        }
        public event Action<ApplicationContainer, InstanceSummary> ApplicationClosing = delegate { };
    }

    public class InstanceSummary { public string Summary { get; set; } public string Result { get; set; } }
}
