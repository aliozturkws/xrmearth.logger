using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using XrmEarth.Logger.Connection;
using XrmEarth.Logger.Renderer;
using XrmEarth.Logger.Renderer.Smtp;

namespace XrmEarth.Logger.Logger
{
    public class SmtpLogger : BaseLogger<SmtpConnection, SmtpRenderer>
    {
        public SmtpLogger(SmtpConnection connection)
            : this(connection, new SmtpRenderer())
        {
        }

        public SmtpLogger(SmtpConnection connection, SmtpRenderer renderer)
            : base(connection, renderer, false)
        {
        }

        protected override void OnPush(Dictionary<string, object> keyValuesDictionary)
        {
            var subject = keyValuesDictionary[SmtpRendererBase.SubjectKey] == null ? string.Empty : keyValuesDictionary[SmtpRendererBase.SubjectKey].ToString();
            var body = keyValuesDictionary[SmtpRendererBase.BodyKey] == null ? string.Empty : keyValuesDictionary[SmtpRendererBase.BodyKey].ToString();

            var mail = CreateMail(subject, body);
            var client = CreateClient();
            client.Send(mail);
        }

        protected virtual MailMessage CreateMail(string subject, string body)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(Connection.FromAddress.Item1, Connection.FromAddress.Item2),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            foreach (var toAddress in Connection.ToAddresses)
            {
                mail.To.Add(new MailAddress(toAddress.Item1, toAddress.Item2));
            }

            return mail;
        }

        protected virtual SmtpClient CreateClient()
        {
            var client = new SmtpClient(Connection.Host, Connection.Port)
            {
                Credentials = new NetworkCredential(Connection.Username, Connection.Password),
                EnableSsl = Connection.EnableSsl
            };

            return client;
        }
    }
}
