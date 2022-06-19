using Microsoft.Xrm.Sdk;
using System;
using System.Text;
using XrmEarth.Logger;
using XrmEarth.Logger.Common;
using XrmEarth.Logger.Enums;
using XrmEarth.Logger.Exceptions;
using XrmEarth.Samples.Base;
using XrmEarth.Samples.Data;

namespace XrmEarth.Samples.Samples
{
    public class Custom : BaseSample
    {
        private CrmConnection _crmConnection;

        protected override void OnRun()
        {
            #region - INIT -

            #region - EVENTS -

            LogManager.Instance.ApplicationInjectFailedException += InstanceOnApplicationInjectFailedException;
            LogManager.Instance.UnhandledException += InstanceOnUnhandledException;
            LogManager.Instance.ApplicationClosing += InstanceOnApplicationClosing;
            LogManager.Instance.SystemNotify += InstanceOnSystemNotify;

            #endregion - EVENTS -

            #region - LOGGER -
            
            //var authenticator = new CrmAuthenticator("")
            //{
            //    Domain = "",
            //    UserName = "",
            //    Password = "",
            //};
            //var orgService = authenticator.Authenticate();
            //_crmConnection = new CrmConnection(orgService);

            //var crmLogger = (CrmLogger)LogManager.CreateLogger(_crmConnection);

            #endregion - LOGGER -

            #region - REGISTRATION -

            //LogManager.RegisterAll(crmLogger);

            #endregion - REGISTRATION -

            #endregion - INIT -

            #region - CUSTOMIZATION -

            //Çağırılan metodun ismi de loglanmak istenirse ICallerMember interface'i implemente edilmelidir. Bu sayede log yapısı çağıran metodu CallerMember özelliğine atayacaktır.
            //crmLogger.Renderer.Register<DataClass>(new CrmRendererBase { GetLogEntity = RenderDataClassEntity });

            #endregion - CUSTOMIZATION -

            #region - USAGE -

            var dataClass = new DataClass
            {
                ID = 1,
                ActionName = "Custom",
                IsSuccess = true
            };

            LogManager.PushInfo(dataClass);

            #endregion - USAGE -
        }

        private Entity RenderDataClassEntity(object obj)
        {
            var dataClass = obj as DataClass;
            if (dataClass == null) return null;

            //Eğer Application ve ApplicationInstance yapısına bağlanmak istenirse, ApplicatioShared sınıfından gerekli parametrelere erişilebilir.
            var container = ApplicationShared.GetApplicationContainer(_crmConnection);
            return new Entity("new_dataclass")
            {
                Attributes =
                {
                    {"new_name", dataClass.ActionName},
                    {"new_applicationinstanceid", new EntityReference(CrmLogger.ApplicationInstanceLogicalName, container.ApplicationInstance.ID)},
                    {"new_callermember", dataClass.CallerMember },
                    {"new_issuccess", dataClass.IsSuccess},
                    {"new_id", dataClass.ID }
                }
            };
        }

        #region - EVENTS -
        private void InstanceOnSystemNotify(SystemNotifyArgs e)
        {
            Output.WriteLine(
                new StringBuilder()
                    .Append("Mesaj: ").Append(e.Message).AppendLine()
                    .Append("Log Tipi: ").Append(e.Type).AppendLine()
                    .Append("Level: ").Append(e.Level).AppendLine()
                    .Append("Yapılandırma Tamamlandı: ").Append(e.InitializeCompleted).AppendLine()
                    .AppendLine()
                    .ToString());
            if (e.InitializeCompleted)
            {
                //Sistem yüklendi.
                LogManager.Push(e.Message, e.Type, e.Level, "SYSTEM_NOTIFY");
            }
            else
            {
                //Sistem henüz tam yüklenemediği için 'Push' metodu çalışmayabilir.
            }
        }

        private void InstanceOnApplicationClosing(InstanceSummary instanceSummary)
        {
            instanceSummary.Result = "Başarılı";
            instanceSummary.Summary = string.Format("Toplam İçeri Alınan Kayıt Sayısı: {0} | Başarılı İşlem Sayısı: {1} | Başarısız İşlem Sayısı: {2}", 65535, 65280, 255);
        }

        private void InstanceOnApplicationInjectFailedException(Exception exception)
        {
            //Uygulama bilgilerine erişilirken bir hata meydana geldi.
            //Not: Sandbox kullanımları için 'InitConfiguration.InjectApplication' false olarak atanmalı.
        }

        private void InstanceOnUnhandledException(UnhandledException unhandledException)
        {
            Output.WriteLine(unhandledException.ToString());
            if (unhandledException.Type == UnhandledExceptionType.Application)
            {
                //Sistem içerisinde hata meydana geldi.
                if (unhandledException.Object != null)
                {
                    //Sistem içerisinde oluşan hata loglama işleminde meydana gelirse, son gönderilen log nesnesine 'Object' özelliğinden erişebilirsiniz.
                }
            }
            else
            {
                //Sistem dışında hata meydana geldi.
            }
            if (unhandledException.IsTerminating)
            {
                //Uygulama sonlandırılacak.
            }
            else
            {
                //Uygulama devam edebilir.
            }
        }
        #endregion - EVENTS - 
    }
}
