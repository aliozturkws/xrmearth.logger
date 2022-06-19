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
    public class SimpleCrm : BaseSample
    {
        protected override void OnRun()
        {
            #region - INIT -

            #region - EVENTS -

            //Uygulama bilgilerine erişirken bir hata alırsa, bu event ile hatayı görüntüleyebilir veya loglayabilirsiniz.
            LogManager.Instance.ApplicationInjectFailedException += InstanceOnApplicationInjectFailedException;

            //Sistem içerisinde bir hata meydana gelirse, bu event ile hatayı görüntüleyebilir veya loglayabilirsiniz.
            LogManager.Instance.UnhandledException += InstanceOnUnhandledException;

            //Uygulama kapatılırken bu event tetiklenir. 
            //Bu event üzerinden uygulama çalışması ile ilgili genel bilgileri 'Result' ve 'Summary' üzerinde saklayabilirsiniz.
            //Not: 'InitConfiguration' üzerinden 'InjectApplication' false atanırsa bu event tetiklenmez.
            LogManager.Instance.ApplicationClosing += InstanceOnApplicationClosing;

            //Sistem bildirimlerini, bu event ile görüntüleyebilir veya loglayabilirsiniz.
            //Not: 'SystemNotifyArgs.InitializeCompleted' özelliği false ise sistem henüz tamamen yüklenmediği için 'Push' metodu düzgün çalışmayabilir.
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
            //var crmConnection = new CrmConnection(orgService);

            //var crmLogger = LogManager.CreateLogger(crmConnection);

            #endregion - LOGGER -

            #region - REGISTRATION -

            //LogManager.RegisterAll(crmLogger);

            #endregion - REGISTRATION -

            #endregion - INIT -

            #region - USAGE -

            LogManager.Info("Uygulama başladı.", 1);

            /*
             * ...
             * CODE
             * ...
             */
            var data = new DataClass
            {
                ID = 1,
                ActionName = "Simple Sample Action",
                IsSuccess = false,
            };
            /*
             * ...
             * EXCEPTION
             * ...
             */
            LogManager.Error("Kayıt yazma işlemi sırasında bir hata meydana geldi. Hata oluşan sınıfı Tag1 alanında görebilirsiniz.", null, JsonSerializerUtil.Serialize(data), "DataClass");
            /*
             * ...
             * CODE
             * ...
             */

            LogManager.Info("Uygulama kapanıyor.", 2);

            #endregion - USAGE -
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
