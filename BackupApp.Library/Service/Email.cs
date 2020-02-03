using BackupApp.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BackupApp.Library.Service
{
    public class Email : INotification
    {
        private EmailModel _model;

        public Email(EmailModel model)
        {
            _model = model;
        }

        public void Send()
        {
            var email = new EmailAddressAttribute();
            if (email.IsValid(_model.EmailTo) && email.IsValid(_model.EmailFrom))
            {
                //EmailModel model = (EmailModel)(object)t;
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(_model.Host);

                mail.From = new MailAddress(_model.EmailFrom, _model.DisplayName);
                mail.To.Add(_model.EmailTo);
                mail.Subject = _model.Subject;
                mail.Body = _model.Body;

                SmtpServer.Port = _model.Port;
                SmtpServer.Credentials = new NetworkCredential(_model.Username, _model.Password);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }
            else
            {
                throw new Exception("Email address not valid!");
            }
        }
    }
}
